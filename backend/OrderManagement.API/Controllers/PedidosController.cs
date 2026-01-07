using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.API.Application.DTOs;
using OrderManagement.API.Application.Services;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly PedidoService _pedidoService;
    private readonly ILogger<PedidosController> _logger;

    public PedidosController(PedidoService pedidoService, ILogger<PedidosController> logger)
    {
        _pedidoService = pedidoService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var pedidos = await _pedidoService.GetAllAsync();
            return Ok(pedidos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los pedidos");
            return StatusCode(500, new { message = "Error al obtener los pedidos" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var pedido = await _pedidoService.GetByIdAsync(id);

            if (pedido == null)
                return NotFound(new { message = $"Pedido con ID {id} no encontrado" });

            return Ok(pedido);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el pedido con ID: {Id}", id);
            return StatusCode(500, new { message = "Error al obtener el pedido" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PedidoDto pedidoDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var (success, error, data) = await _pedidoService.CreateAsync(pedidoDto);

            if (!success)
                return BadRequest(new { message = error });

            _logger.LogInformation("Pedido creado exitosamente: {NumeroPedido}", data!.NumeroPedido);
            return CreatedAtAction(nameof(GetById), new { id = data.Id }, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el pedido");
            return StatusCode(500, new { message = "Error al crear el pedido" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PedidoDto pedidoDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var (success, error, data) = await _pedidoService.UpdateAsync(id, pedidoDto);

            if (!success)
            {
                if (error == "Pedido no encontrado")
                    return NotFound(new { message = error });

                return BadRequest(new { message = error });
            }

            _logger.LogInformation("Pedido actualizado exitosamente: {NumeroPedido}", data!.NumeroPedido);
            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el pedido con ID: {Id}", id);
            return StatusCode(500, new { message = "Error al actualizar el pedido" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _pedidoService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { status = "error", message = $"Pedido con ID {id} no encontrado" });

            _logger.LogInformation("Pedido eliminado exitosamente: ID {Id}", id);
            return Ok(new { status = "success", message = "Pedido eliminado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el pedido con ID: {Id}", id);
            return StatusCode(500, new { status = "error", message = "Error al eliminar el pedido" });
        }
    }
}
