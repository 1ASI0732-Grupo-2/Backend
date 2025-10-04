namespace workstation_backend.UserContext.Domain.Models.Commands;

/// <summary>
/// Comando para eliminar un usuario del sistema.
/// </summary>
/// <param name="Id">Identificador único del usuario.</param>
public record DeleteUserCommand(Guid Id);
