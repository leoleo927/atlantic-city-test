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
            .Where(p => !p.Eliminado)
            .OrderByDescending(p => p.Fecha)
            .ToListAsync();
    }

    public async Task<Pedido?> GetByIdAsync(int id)
    {
        return await _context.Pedidos
            .FirstOrDefaultAsync(p => p.Id == id && !p.Eliminado);
    }

    public async Task<Pedido?> GetByNumeroPedidoAsync(string numeroPedido)
    {
        return await _context.Pedidos
            .FirstOrDefaultAsync(p => p.NumeroPedido == numeroPedido && !p.Eliminado);
    }

    public async Task<Pedido> CreateAsync(Pedido pedido)
    {
        pedido.Fecha = DateTime.UtcNow;
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();
        return pedido;
    }

    public async Task<Pedido> UpdateAsync(Pedido pedido)
    {
        _context.Entry(pedido).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return pedido;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido == null)
            return false;

        // Eliminación lógica
        pedido.Eliminado = true;
        pedido.FechaEliminacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExisteNumeroPedidoAsync(string numeroPedido, int? excludeId = null)
    {
        var query = _context.Pedidos.Where(p => p.NumeroPedido == numeroPedido && !p.Eliminado);

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
