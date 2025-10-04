using workstation_backend.UserContext.Domain.Models.Entities;
namespace workstation_backend.UserContext.Domain.Models.Commands;

/// <summary>
/// Comando para crear un nuevo usuario en la plataforma.
/// </summary>
/// <param name="FirstName">Nombre del usuario.</param>
/// <param name="LastName">Apellido del usuario.</param>
/// <param name="Dni">Número de documento de identidad.</param>
/// <param name="PhoneNumber">Número de teléfono de contacto.</param>
/// <param name="Email">Correo electrónico del usuario.</param>
/// <param name="Role">Rol asignado al usuario (ej. Buscador, Alquilador).</param>
/// <param name="Nickname">Apodo o alias del usuario (opcional).</param>
/// <param name="Description">Descripción personal del usuario (opcional).</param>
/// <param name="BusinessName">Nombre del negocio del usuario (opcional).</param>
/// <param name="ExtraInfo">Información adicional relevante (opcional).</param>
public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Dni,
    string PhoneNumber,
    string Email,
    String PasswordHash,
    UserRole Role,
    string? Nickname,       
    string? Description,    
    string? BusinessName,   
    string? ExtraInfo       
);
