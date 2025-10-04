using System;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using workstation_backend.UserContext.Domain.Models.Commands;
using workstation_backend.UserContext.Domain.Models.Entities;
using workstation_backend.UserContext.Domain.Models.Exceptions;
using workstation_backend.UserContext.Domain.Models.Queries;
using workstation_backend.UserContext.Domain.Services;
using workstation_backend.UserContext.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;

namespace workstation_backend.UserContext.Interfaces.REST;

/// <summary>
/// Controlador para la gestión de usuarios del sistema
/// </summary>
[Route("api/workstation/[controller]")]
[ApiController]
[Produces("application/json")]
public class UserController(IUserQueryService userQueryService, IUserCommandService userCommandService) : ControllerBase
{
    private readonly IUserQueryService _userQueryService = userQueryService ?? throw new ArgumentNullException(nameof(userQueryService));
    private readonly IUserCommandService _userCommandService = userCommandService ?? throw new ArgumentNullException(nameof(userCommandService));

    /// <summary>
    /// Obtiene todos los usuarios del sistema
    /// </summary>
    /// <returns>Lista de usuarios registrados</returns>
    /// <response code="200">Retorna la lista de usuarios</response>
    /// <response code="404">No se encontraron usuarios</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        try
        {
            var query = new GetAllUsersQuery();
            var result = await _userQueryService.Handle(query);

            if (!result.Any()) return NotFound("No users found.");

            var resourcers = result.Select(UserResourceFromEntityAssembler.ToResourceFromEntity).ToList();
            return Ok(resourcers);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Obtiene un usuario por su ID único
    /// </summary>
    /// <param name="id">ID único del usuario (GUID)</param>
    /// <returns>Datos del usuario solicitado</returns>
    /// <response code="200">Retorna el usuario encontrado</response>
    /// <response code="400">ID de usuario inválido</response>
    /// <response code="404">Usuario no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserById([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Invalid user ID.");

        try
        {
            var query = new GetUserByIdQuery(id);
            var result = await _userQueryService.Handle(query);

            return result != null
                ? Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(result))
                : NotFound($"User with ID {id} not found.");
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Crea un nuevo usuario en el sistema
    /// </summary>
    /// <param name="command">Datos del usuario a crear</param>
    /// <returns>Confirmación de creación del usuario</returns>
    /// <response code="201">Usuario creado exitosamente</response>
    /// <response code="400">Datos de entrada inválidos o usuario no encontrado</response>
    /// <response code="409">Ya existe un usuario con el mismo DNI</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("sign-up")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        if (command == null)
            return BadRequest("Invalid user data.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _userCommandService.Handle(command);
            return StatusCode(StatusCodes.Status201Created, "User created successfully.");
        }
        catch (UserNotFoundException exception)
        {
            return BadRequest(exception.Message);
        }
        catch (DuplicateNameException)
        {
            return Conflict("A user with the same DNI already exists.");
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Actualiza los datos de un usuario existente
    /// </summary>
    /// <param name="id">ID único del usuario a actualizar</param>
    /// <param name="updateUserCommand">Nuevos datos del usuario</param>
    /// <returns>Confirmación de actualización</returns>
    /// <response code="200">Usuario actualizado exitosamente</response>
    /// <response code="400">ID inválido o datos de entrada incorrectos</response>
    /// <response code="404">Usuario no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UpdateUserCommand updateUserCommand)
    {
        if (id == Guid.Empty)
            return BadRequest("Invalid user ID.");

        if (updateUserCommand == null)
            return BadRequest("Invalid user data.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _userCommandService.Handle(updateUserCommand, id);
            return Ok("User updated successfully.");
        }
        catch (UserNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Elimina un usuario del sistema
    /// </summary>
    /// <param name="id">ID único del usuario a eliminar</param>
    /// <returns>Confirmación de eliminación</returns>
    /// <response code="204">Usuario eliminado exitosamente</response>
    /// <response code="400">ID de usuario inválido</response>
    /// <response code="404">Usuario no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("Invalid user ID.");

        try
        {
            var command = new DeleteUserCommand(id);
            await _userCommandService.Handle(command);
            return NoContent();
        }
        catch (UserNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        if (command == null)
            return BadRequest("Invalid login data.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var jwToken = await userCommandService.Handle(command);
            return Ok(jwToken);
        }
        catch (UserNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}