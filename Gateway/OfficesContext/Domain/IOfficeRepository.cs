using System;
using workstation_backend.OfficesContext.Domain.Models.Entities;
using workstation_backend.Shared.Domain.Repositories;

namespace workstation_backend.OfficesContext.Domain;

/// <summary>
/// Define las operaciones de persistencia específicas para entidades <see cref="Office"/>.
/// </summary>
public interface IOfficeRepository : IBaseRepository<Office>
{
    /// <summary>
    /// Obtiene una oficina por su ubicación.
    /// </summary>
    /// <param name="location">Nombre o descripción de la ubicación (ej. Miraflores, San Isidro).</param>
    /// <returns>La oficina correspondiente o null si no se encuentra.</returns>
    Task<Office?> GetByLocationAsync(string location);
}
