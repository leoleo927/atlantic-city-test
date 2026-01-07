using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Domain.Entities;
using OrderManagement.API.Domain.Interfaces;
using OrderManagement.API.Infrastructure.Data;

namespace OrderManagement.API.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly ApplicationDbContext _context;

    public PedidoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Pedido>> GetAllAsync()
    {
        return await _context.Pedidos
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    public async Task<Pedido?> GetByIdAsync(int id)
    {
        return await _context.Pedidos
            .FirstOrDefaultAsync(p => p.Id == id && p.Estado != "Eliminado");
    }

    public async Task<Pedido?> GetByNumeroPedidoAsync(string numeroPedido)
    {
        return await _context.Pedidos
            .FirstOrDefaultAsync(p => p.NumeroPedido == numeroPedido && p.Estado != "Eliminado");
    }

    public async Task<Pedido> CreateAsync(Pedido pedido)
    {
        var now = DateTime.UtcNow;
        pedido.FechaCreacion = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        pedido.FechaModificacion = null;
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();
        return pedido;
    }

    public async Task<Pedido> UpdateAsync(Pedido pedido)
    {
        var now = DateTime.UtcNow;
        pedido.FechaModificacion = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        _context.Entry(pedido).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return pedido;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido == null || pedido.Estado == "Eliminado")
            return false;

        // Cambiar estado a "Eliminado" y registrar fecha de modificación
        pedido.Estado = "Eliminado";
        var now = DateTime.UtcNow;
        pedido.FechaModificacion = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string> GenerarNumeroPedidoAsync()
    {
        // Obtener el último número de pedido (incluyendo eliminados)
        var ultimoPedido = await _context.Pedidos
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync();

        if (ultimoPedido == null)
        {
            return "PED-001";
        }

        // Extraer el número del último pedido
        var numeroActual = ultimoPedido.NumeroPedido;

        // Si tiene el formato PED-XXX, extraer el número
        if (numeroActual.StartsWith("PED-"))
        {
            var partes = numeroActual.Split('-');
            if (partes.Length >= 2 && int.TryParse(partes[1], out int numero))
            {
                var siguienteNumero = numero + 1;
                return $"PED-{siguienteNumero:D3}";
            }
        }

        // Si no tiene el formato esperado, usar el ID
        return $"PED-{ultimoPedido.Id + 1:D3}";
    }

    public async Task<bool> ExisteNumeroPedidoAsync(string numeroPedido, int? excludeId = null)
    {
        var query = _context.Pedidos.Where(p => p.NumeroPedido == numeroPedido && p.Estado != "Eliminado");

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
