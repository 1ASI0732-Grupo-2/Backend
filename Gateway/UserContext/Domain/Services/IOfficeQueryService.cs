using System;
using workstation_backend.UserContext.Domain.Models.Entities;
using workstation_backend.UserContext.Domain.Models.Queries;

namespace workstation_backend.UserContext.Domain.Services;

/// <summary>
/// Define las operaciones de consulta relacionadas con usuarios.
/// </summary>
public interface IUserQueryService
{
    /// <summary>
    /// Obtiene todos los usuarios registrados en el sistema.
    /// </summary>
    /// <param name="query">Consulta vacía o con filtros generales (si los hay).</param>
    /// <returns>Colección de usuarios.</returns>
    Task<IEnumerable<User>> Handle(GetAllUsersQuery query);

    /// <summary>
    /// Obtiene la información de un usuario específico por su ID.
    /// </summary>
    /// <param name="query">Consulta que contiene el identificador del usuario.</param>
    /// <returns>El usuario correspondiente, si existe.</returns>
    Task<User> Handle(GetUserByIdQuery query);
}
