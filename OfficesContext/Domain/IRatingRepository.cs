using workstation_backend.Shared.Domain.Repositories;

namespace workstation_backend.OfficesContext.Domain;

using OfficesContext.Domain.Models.Entities;

/// <summary>
/// Define las operaciones de persistencia específicas para entidades <see cref="Rating"/>.
/// </summary>
public interface IRatingRepository : IBaseRepository<Rating>
{
    /// <summary>
    /// Obtiene todas las calificaciones asociadas a una oficina.
    /// </summary>
    /// <param name="officeId">Identificador de la oficina.</param>
    /// <returns>Una colección de calificaciones.</returns>
    Task<IEnumerable<Rating>> GetByOfficeIdAsync(Guid officeId);
}
