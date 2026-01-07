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

        // Validación: Número de pedido único
        if (await _pedidoRepository.ExisteNumeroPedidoAsync(dto.NumeroPedido))
            return (false, $"Ya existe un pedido con el número '{dto.NumeroPedido}'", null);

        var pedido = new Pedido
        {
            NumeroPedido = dto.NumeroPedido,
            Cliente = dto.Cliente,
            Total = dto.Total,
            Estado = dto.Estado,
            Fecha = DateTime.UtcNow
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

        // Validación: Número de pedido único (excluyendo el actual)
        if (await _pedidoRepository.ExisteNumeroPedidoAsync(dto.NumeroPedido, id))
            return (false, $"Ya existe un pedido con el número '{dto.NumeroPedido}'", null);

        existingPedido.NumeroPedido = dto.NumeroPedido;
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
            Fecha = pedido.Fecha,
            Total = pedido.Total,
            Estado = pedido.Estado
        };
    }
}
