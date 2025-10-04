using System;
using workstation_backend.UserContext.Domain;
using workstation_backend.UserContext.Domain.Models.Entities;
using workstation_backend.UserContext.Domain.Models.Queries;
using workstation_backend.UserContext.Domain.Services;

namespace workstation_backend.UserContext.Application.QueryServices;

/// <summary>
/// Servicio encargado de manejar consultas relacionadas con usuarios.
/// Implementa operaciones para obtener todos los usuarios o uno específico por ID.
/// </summary>
public class UserQueryService(IUserRepository userRepository) : IUserQueryService
{
    // Repositorio de usuarios para acceder a datos persistentes.
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    /// <summary>
    /// Maneja la consulta para obtener todos los usuarios registrados.
    /// </summary>
    /// <param name="query">Consulta vacía o genérica.</param>
    /// <returns>Una colección de usuarios.</returns>
    public async Task<IEnumerable<User>> Handle(GetAllUsersQuery query)
    {
        var users = await _userRepository.GetAllAsync();
        return users;
    }

    /// <summary>
    /// Maneja la consulta para obtener un usuario por su identificador único.
    /// </summary>
    /// <param name="query">Consulta que contiene el ID del usuario.</param>
    /// <returns>El usuario correspondiente si existe; null si no se encuentra.</returns>
    /// <exception cref="ArgumentNullException">Si la consulta es nula.</exception>
    public async Task<User?> Handle(GetUserByIdQuery query)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));

        var user = await _userRepository.FindByIdAsync(query.UserId);
        return user;
    }
}




    

