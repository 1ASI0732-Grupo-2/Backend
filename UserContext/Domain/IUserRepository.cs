using System;
using workstation_backend.UserContext.Domain.Models.Entities;
using workstation_backend.Shared.Domain.Repositories;

namespace workstation_backend.UserContext.Domain;

/// <summary>
/// Define las operaciones de persistencia específicas para entidades <see cref="User"/>.
/// </summary>
public interface IUserRepository : IBaseRepository<User>
{
    /// <summary>
    /// Obtiene todos los usuarios registrados.
    /// </summary>
    /// <returns>Una colección de usuarios.</returns>
    Task<IEnumerable<User>> GetAllAsync();

    /// <summary>
    /// Obtiene un usuario por su email.
    /// </summary>
    /// <param name="email">email que se uso para registrarse</param>
    /// <returns>El usuario correspondiente o null si no se encuentra.</returns>
    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByPhoneNumberAsync(string phoneNumber);

    /// <summary>
    /// Obtiene un usuario por su número de DNI.
    /// </summary>
    /// <param name="dni">Número de documento nacional de identidad.</param>
    /// <returns>El usuario correspondiente o null si no se encuentra.</returns>
    Task<User?> GetByDniAsync(string dni);
}
