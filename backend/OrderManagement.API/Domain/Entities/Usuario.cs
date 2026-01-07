namespace OrderManagement.API.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Rol { get; set; } = "User"; // Admin, User
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
