using System.ComponentModel.DataAnnotations;

namespace OrderManagement.API.Application.DTOs;

public class PedidoDto
{
    public int Id { get; set; }

    [StringLength(50, ErrorMessage = "El número de pedido no puede exceder 50 caracteres")]
    public string NumeroPedido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El cliente es requerido")]
    [StringLength(150, ErrorMessage = "El nombre del cliente no puede exceder 150 caracteres")]
    public string Cliente { get; set; } = string.Empty;

    public string FechaCreacion { get; set; } = string.Empty;

    public string? FechaModificacion { get; set; }

    [Required(ErrorMessage = "El total es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor que 0")]
    public decimal Total { get; set; }

    [Required(ErrorMessage = "El estado es requerido")]
    [StringLength(50, ErrorMessage = "El estado no puede exceder 50 caracteres")]
    public string Estado { get; set; } = string.Empty;
}
