using System;
using Microsoft.EntityFrameworkCore;
using workstation_backend.UserContext.Domain;
using workstation_backend.UserContext.Domain.Models.Entities;
using workstation_backend.Shared.Infrastructure.Persistence.Configuration;
using workstation_backend.Shared.Infrastructure.Persistence.Repositories;
using workstation_backend.Shared.Domain;

namespace workstation_backend.UserContext.Infrastructure;

/// <summary>
/// Repositorio para operaciones relacionadas con la entidad <see cref="User"/>.
/// Hereda de <see cref="BaseRepository{User}"/> e implementa <see cref="IUserRepository"/>.
/// </summary>
public class UserRepository(WorkstationContext context) 
    : BaseRepository<User>(context), IUserRepository
{
    /// <summary>
    /// Obtiene un usuario por su número de DNI.
    /// </summary>
    /// <param name="dni">DNI del usuario.</param>
    /// <returns>El usuario si se encuentra; null si no existe.</returns>
    public async Task<User?> GetByDniAsync(string dni)
    {
        return await Context.Set<User>().FirstOrDefaultAsync(user => user.Dni == dni);
    }
    /// <summary>
    /// Obtiene un usuario por su dirección de correo electrónico.
    /// </summary>
    /// <param name="email">Correo electrónico del usuario.</param>
    /// <returns>El usuario si se encuentra; null si no existe.</returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await Context.Set<User>().FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await Context.Set<User>().FirstOrDefaultAsync(user => user.PhoneNumber == phoneNumber);
    }

    /// <summary>
    /// Retorna todos los usuarios registrados en el sistema.
    /// </summary>
    /// <returns>Una colección de usuarios.</returns>
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await Context.Set<User>().ToListAsync();
    }
}
