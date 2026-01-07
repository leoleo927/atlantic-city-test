using OrderManagement.API.Domain.Entities;

namespace OrderManagement.API.Domain.Interfaces;

public interface IPedidoRepository
{
    Task<IEnumerable<Pedido>> GetAllAsync();
    Task<Pedido?> GetByIdAsync(int id);
    Task<Pedido?> GetByNumeroPedidoAsync(string numeroPedido);
    Task<Pedido> CreateAsync(Pedido pedido);
    Task<Pedido> UpdateAsync(Pedido pedido);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExisteNumeroPedidoAsync(string numeroPedido, int? excludeId = null);
}
