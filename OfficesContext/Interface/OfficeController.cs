using System;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using workstation_backend.OfficesContext.Domain.Models.Commands;
using workstation_backend.OfficesContext.Domain.Models.Exceptions;
using workstation_backend.OfficesContext.Domain.Models.Queries;
using workstation_backend.OfficesContext.Domain.Services;
using workstation_backend.OfficesContext.Interface.Transform;

namespace workstation_backend.OfficesContext.Interface;

/// <summary>
/// Controlador para la gestión de oficinas y espacios de trabajo
/// </summary>
[Route("api/workstation/[controller]")]
[ApiController]
[Produces("application/json")]
public class OfficeController(IOfficeQueryService officeQueryService, IOfficeCommandService officeCommandService) : ControllerBase
{
    private readonly IOfficeQueryService _officeQueryService = officeQueryService ?? throw new ArgumentNullException(nameof(officeQueryService));
    private readonly IOfficeCommandService _officeCommandService = officeCommandService ?? throw new ArgumentNullException(nameof(officeCommandService));

    /// <summary>
    /// Obtiene todas las oficinas disponibles en el sistema
    /// </summary>
    /// <returns>Lista de oficinas registradas</returns>
    /// <response code="200">Retorna la lista de oficinas</response>
    /// <response code="404">No se encontraron oficinas</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllOfficesAsync()
    {
        try
        {
            var query = new GetAllOfficesQuery();
            var result = await _officeQueryService.Handle(query);

            if (!result.Any()) return NotFound("No offices found.");

            var resources = result.Select(OfficeResourceFromEntityAssembler.ToResourceFromEntity).ToList();
            return Ok(resources);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Obtiene una oficina específica por su ID único
    /// </summary>
    /// <param name="id">ID único de la oficina (GUID)</param>
    /// <returns>Datos de la oficina solicitada</returns>
    /// <response code="200">Retorna la oficina encontrada</response>
    /// <response code="400">ID de oficina inválido</response>
    /// <response code="404">Oficina no encontrada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOfficeByIdAsync([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Invalid office ID.");

        try
        {
            var query = new GetOfficeByIdQuery(id);
            var result = await _officeQueryService.Handle(query);

            return result != null 
                ? Ok(OfficeResourceFromEntityAssembler.ToResourceFromEntity(result)) 
                : NotFound($"Office with ID {id} not found.");
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Busca oficinas por ubicación específica
    /// </summary>
    /// <param name="location">Ubicación o dirección de la oficina</param>
    /// <returns>Oficina encontrada en la ubicación especificada</returns>
    /// <response code="200">Retorna la oficina encontrada en la ubicación</response>
    /// <response code="400">Ubicación inválida o vacía</response>
    /// <response code="404">No se encontró oficina en esa ubicación</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("by-location/{location}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOfficeByLocationAsync([FromRoute] string location)
    {
        if (string.IsNullOrWhiteSpace(location)) 
            return BadRequest("Invalid location. Location cannot be empty.");

        try
        {
            var query = new GetOfficeByLocation(location);
            var result = await _officeQueryService.Handle(query);

            return result != null 
                ? Ok(OfficeResourceFromEntityAssembler.ToResourceFromEntity(result)) 
                : NotFound($"Office with location '{location}' not found.");
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Crea una nueva oficina en el sistema
    /// </summary>
    /// <param name="command">Datos de la oficina a crear</param>
    /// <returns>Confirmación de creación de la oficina</returns>
    /// <response code="201">Oficina creada exitosamente</response>
    /// <response code="400">Datos de entrada inválidos o servicios no encontrados</response>
    /// <response code="409">Ya existe una oficina en la misma ubicación</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOfficeAsync([FromBody] CreateOfficeCommand command)
    {
        if (command == null) 
        return BadRequest("Invalid office data.");

    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    try
    {
        await _officeCommandService.Handle(command);
        return StatusCode(StatusCodes.Status201Created, "Office created successfully.");
    }
    catch (FluentValidation.ValidationException ex)
    {
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        return BadRequest(new { errors });
    }
    catch (NotServicesFoundException exception)
    {
        return BadRequest(new { errors = new { Services = new[] { exception.Message } } });
    }
    catch (DuplicateNameException)
    {
        return Conflict(new { errors = new { Location = new[] { "An office with the same location already exists." } } });
    }
    catch (Exception ex)
    {
        return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
    }

    /// <summary>
    /// Actualiza los datos de una oficina existente
    /// </summary>
    /// <param name="id">ID único de la oficina a actualizar</param>
    /// <param name="updateOfficeCommand">Nuevos datos de la oficina</param>
    /// <returns>Confirmación de actualización</returns>
    /// <response code="200">Oficina actualizada exitosamente</response>
    /// <response code="400">ID inválido o datos de entrada incorrectos</response>
    /// <response code="404">Oficina no encontrada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateOfficeAsync([FromRoute] Guid id, [FromBody] UpdateOfficeCommand updateOfficeCommand)
    {
        if (id == Guid.Empty)
            return BadRequest("Invalid office ID.");

        if (updateOfficeCommand == null)
            return BadRequest("Invalid office data.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _officeCommandService.Handle(updateOfficeCommand, id);
            return Ok("Office updated successfully.");
        }
        catch (OfficeNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Elimina una oficina del sistema
    /// </summary>
    /// <param name="id">ID único de la oficina a eliminar</param>
    /// <returns>Confirmación de eliminación</returns>
    /// <response code="204">Oficina eliminada exitosamente</response>
    /// <response code="400">ID de oficina inválido</response>
    /// <response code="404">Oficina no encontrada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteOfficeAsync([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Invalid office ID.");

        try
        {
            var command = new DeleteOfficeCommand(id);
            await _officeCommandService.Handle(command);
            return NoContent();
        }
        catch (OfficeNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}