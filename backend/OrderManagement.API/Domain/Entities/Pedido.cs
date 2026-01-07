namespace OrderManagement.API.Domain.Entities;

public class Pedido
{
    public int Id { get; set; }
    public string NumeroPedido { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = string.Empty; // Registrado, En Proceso, Completado, Eliminado
}
