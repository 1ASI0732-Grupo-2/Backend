using System;
using workstation_backend.UserContext.Domain.Models.Commands;
using workstation_backend.UserContext.Domain.Models.Entities;

namespace workstation_backend.UserContext.Domain.Services;

/// <summary>
/// Define las operaciones de comando relacionadas con la gestión de usuarios (crear, actualizar, eliminar).
/// </summary>
public interface IUserCommandService
{
    /// <summary>
    /// Crea un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="command">Comando con los datos del nuevo usuario.</param>
    /// <returns>El usuario creado.</returns>
    Task<User> Handle(CreateUserCommand command);

    /// <summary>
    /// Actualiza los datos de un usuario existente.
    /// </summary>
    /// <param name="command">Comando con los datos actualizados del usuario.</param>
    /// <param name="id">Identificador del usuario a actualizar.</param>
    /// <returns>True si la actualización fue exitosa, false si no.</returns>
    Task<bool> Handle(UpdateUserCommand command, Guid id);

    /// <summary>
    /// Elimina un usuario del sistema.
    /// </summary>
    /// <param name="command">Comando con el identificador del usuario a eliminar.</param>
    Task Handle(DeleteUserCommand command);

    Task<string> Handle(LoginCommand command);
}


