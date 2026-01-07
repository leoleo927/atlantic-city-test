namespace OrderManagement.API.Application.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
