using System;
using Microsoft.EntityFrameworkCore;
using workstation_backend.OfficesContext.Domain;
using workstation_backend.OfficesContext.Domain.Models.Entities;
using workstation_backend.Shared.Infrastructure.Persistence.Configuration;
using workstation_backend.Shared.Infrastructure.Persistence.Repositories;

namespace workstation_backend.OfficesContext.Infrastructure;

/// <summary>
/// Repositorio para operaciones relacionadas con la entidad <see cref="Office"/>.
/// Hereda de <see cref="BaseRepository{Office}"/> e implementa <see cref="IOfficeRepository"/>.
/// </summary>
public class OfficeRepository(WorkstationContext context) : BaseRepository<Office>(context), IOfficeRepository
{
    /// <summary>
    /// Obtiene una oficina por su ubicación exacta.
    /// </summary>
    /// <param name="location">Ubicación de la oficina.</param>
    /// <returns>La oficina correspondiente si existe; null si no se encuentra.</returns>
    public async Task<Office?> GetByLocationAsync(string location)
    {
        return await context.Set<Office>().FirstOrDefaultAsync(office => office.Location == location);
    }
    public new async Task<IEnumerable<Office>> ListAsync()
{
    return await Context.Offices
        .Include(o => o.Services)
        .ToListAsync();
}
}
