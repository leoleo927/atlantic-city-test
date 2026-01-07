using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OrderManagement.API.Application.DTOs;
using OrderManagement.API.Domain.Entities;
using OrderManagement.API.Domain.Interfaces;
using BCrypt.Net;

namespace OrderManagement.API.Application.Services;

public class AuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        // Buscar usuario por email
        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
        if (usuario == null)
            return null;

        // Verificar contraseña
        if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
            return null;

        // Generar JWT Token
        var token = GenerateJwtToken(usuario);
        var expirationHours = int.Parse(_configuration["JwtSettings:ExpirationInHours"] ?? "24");

        return new LoginResponseDto
        {
            Token = token,
            ExpiresIn = expirationHours * 3600, // en segundos
            Email = usuario.Email,
            Nombre = usuario.Nombre,
            Rol = usuario.Rol
        };
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        var secret = _configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
        var issuer = _configuration["JwtSettings:Issuer"];
        var audience = _configuration["JwtSettings:Audience"];
        var expirationHours = int.Parse(_configuration["JwtSettings:ExpirationInHours"] ?? "24");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expirationHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<Usuario?> RegisterAsync(string email, string password, string nombre, string rol = "User")
    {
        // Verificar si el usuario ya existe
        var existingUser = await _usuarioRepository.GetByEmailAsync(email);
        if (existingUser != null)
            return null;

        // Hashear la contraseña
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var usuario = new Usuario
        {
            Email = email,
            PasswordHash = passwordHash,
            Nombre = nombre,
            Rol = rol,
            FechaCreacion = DateTime.UtcNow
        };

        return await _usuarioRepository.CreateAsync(usuario);
    }
}
