namespace OrderManagement.API.Domain.Entities;

public class Pedido
{
    public int Id { get; set; }
    public string NumeroPedido { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = string.Empty;
    public bool Eliminado { get; set; } = false; // Para eliminación lógica
    public DateTime? FechaEliminacion { get; set; }
}
