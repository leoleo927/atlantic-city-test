using Microsoft.AspNetCore.Mvc;
using OrderManagement.API.Application.DTOs;
using OrderManagement.API.Application.Services;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var response = await _authService.LoginAsync(request);

            if (response == null)
            {
                _logger.LogWarning("Intento de login fallido para el email: {Email}", request.Email);
                return Unauthorized(new { message = "Email o contraseña incorrectos" });
            }

            _logger.LogInformation("Login exitoso para el usuario: {Email}", request.Email);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el login para el email: {Email}", request.Email);
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
