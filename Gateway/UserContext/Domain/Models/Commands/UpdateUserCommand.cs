using workstation_backend.UserContext.Domain.Models.Entities;

namespace workstation_backend.UserContext.Domain.Models.Commands;

/// <summary>
/// Comando para actualizar los datos de un usuario existente.
/// </summary>
/// <param name="UserId">Identificador único del usuario.</param>
/// <param name="FirstName">Nuevo nombre del usuario.</param>
/// <param name="LastName">Nuevo apellido del usuario.</param>
/// <param name="Dni">Nuevo número de documento de identidad.</param>
/// <param name="PhoneNumber">Nuevo número de teléfono.</param>
/// <param name="Email">Nuevo correo electrónico.</param>
/// <param name="role">Nuevo rol asignado al usuario.</param>
public record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Dni,
    string PhoneNumber,
    string Email,
    UserRole role
);

