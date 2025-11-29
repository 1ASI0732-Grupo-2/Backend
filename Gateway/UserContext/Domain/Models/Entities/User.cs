using System;
using workstation_backend.Shared.Domain.Model;

namespace workstation_backend.UserContext.Domain.Models.Entities;

/// <summary>
/// Representa un usuario de la plataforma, ya sea arrendatario o buscador.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Constructor para crear un nuevo usuario.
    /// </summary>
    /// <param name="firstName">Nombre del usuario.</param>
    /// <param name="lastName">Apellido del usuario.</param>
    /// <param name="dni">Documento nacional de identidad.</param>
    /// <param name="phoneNumber">Número de teléfono.</param>
    /// <param name="email">Correo electrónico del usuario.</param>
    /// <param name="role">Rol asignado al usuario.</param>
    public User(string firstName, string lastName, string dni, string phoneNumber, string email, UserRole role, String PasswordHash)
    {
        FirstName = firstName;
        LastName = lastName;
        Dni = dni;
        PhoneNumber = phoneNumber;
        Email = email;
        Role = role;
        CreatedAt = DateTime.UtcNow;
        this.PasswordHash = PasswordHash;
    }

    /// <summary>
    /// Nombre del usuario.
    /// </summary>
    public string FirstName { get; private set; }

    /// <summary>
    /// Apellido del usuario.
    /// </summary>
    public string LastName { get; private set; }

    /// <summary>
    /// Número de documento de identidad.
    /// </summary>
    public string Dni { get; private set; }

    /// <summary>
    /// Número de teléfono de contacto.
    /// </summary>
    public string PhoneNumber { get; private set; }

    /// <summary>
    /// Dirección de correo electrónico.
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// Rol del usuario (ej. arrendador o buscador).
    /// </summary>
    public UserRole Role { get; private set; }

    /// <summary>
    /// Fecha y hora de creación del usuario (UTC).
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    public String PasswordHash { get; private set; }

    /// <summary>
    /// Actualiza los datos personales del usuario.
    /// </summary>
    /// <param name="firstName">Nuevo nombre.</param>
    /// <param name="lastName">Nuevo apellido.</param>
    /// <param name="phoneNumber">Nuevo teléfono.</param>
    /// <param name="email">Nuevo correo electrónico.</param>
    public void Update(string firstName, string lastName, string phoneNumber, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
    }
}
