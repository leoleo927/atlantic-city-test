using OrderManagement.API.Application.DTOs;
using OrderManagement.API.Domain.Entities;
using OrderManagement.API.Domain.Interfaces;

namespace OrderManagement.API.Application.Services;

public class PedidoService
{
    private readonly IPedidoRepository _pedidoRepository;

    public PedidoService(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    public async Task<IEnumerable<PedidoDto>> GetAllAsync()
    {
        var pedidos = await _pedidoRepository.GetAllAsync();
        return pedidos.Select(MapToDto);
    }

    public async Task<PedidoDto?> GetByIdAsync(int id)
    {
        var pedido = await _pedidoRepository.GetByIdAsync(id);
        return pedido != null ? MapToDto(pedido) : null;
    }

    public async Task<(bool Success, string? Error, PedidoDto? Data)> CreateAsync(PedidoDto dto)
    {
        // Validación: Total mayor que 0
        if (dto.Total <= 0)
            return (false, "El total debe ser mayor que 0", null);

        // Generar automáticamente el número de pedido
        var numeroPedido = await _pedidoRepository.GenerarNumeroPedidoAsync();

        var pedido = new Pedido
        {
            NumeroPedido = numeroPedido,
            Cliente = dto.Cliente,
            Total = dto.Total,
            Estado = dto.Estado
        };

        var createdPedido = await _pedidoRepository.CreateAsync(pedido);
        return (true, null, MapToDto(createdPedido));
    }

    public async Task<(bool Success, string? Error, PedidoDto? Data)> UpdateAsync(int id, PedidoDto dto)
    {
        var existingPedido = await _pedidoRepository.GetByIdAsync(id);
        if (existingPedido == null)
            return (false, "Pedido no encontrado", null);

        // Validación: Total mayor que 0
        if (dto.Total <= 0)
            return (false, "El total debe ser mayor que 0", null);

        // NO permitir editar el número de pedido - se mantiene el original
        // existingPedido.NumeroPedido NO se modifica
        existingPedido.Cliente = dto.Cliente;
        existingPedido.Total = dto.Total;
        existingPedido.Estado = dto.Estado;

        var updatedPedido = await _pedidoRepository.UpdateAsync(existingPedido);
        return (true, null, MapToDto(updatedPedido));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _pedidoRepository.DeleteAsync(id);
    }

    private static PedidoDto MapToDto(Pedido pedido)
    {
        return new PedidoDto
        {
            Id = pedido.Id,
            NumeroPedido = pedido.NumeroPedido,
            Cliente = pedido.Cliente,
            FechaCreacion = pedido.FechaCreacion.ToString("yyyy-MM-dd HH:mm"),
            FechaModificacion = pedido.FechaModificacion?.ToString("yyyy-MM-dd HH:mm"),
            Total = pedido.Total,
            Estado = pedido.Estado
        };
    }
}
