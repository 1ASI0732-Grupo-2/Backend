using System;
using workstation_backend.OfficesContext.Domain.Models.Entities;
using workstation_backend.OfficesContext.Domain.Models.Queries;

namespace workstation_backend.OfficesContext.Domain.Services;

/// <summary>
/// Define las operaciones de consulta para obtener información sobre oficinas.
/// </summary>
public interface IOfficeQueryService
{
    /// <summary>
    /// Obtiene la lista de todas las oficinas registradas.
    /// </summary>
    /// <param name="query">Parámetros de consulta, si los hay.</param>
    /// <returns>Una colección de oficinas.</returns>
    Task<IEnumerable<Office>> Handle(GetAllOfficesQuery query);

    /// <summary>
    /// Obtiene una oficina específica por su identificador.
    /// </summary>
    /// <param name="query">Consulta con el identificador de la oficina.</param>
    /// <returns>La oficina correspondiente, si existe.</returns>
    Task<Office> Handle(GetOfficeByIdQuery query);

    /// <summary>
    /// Obtiene una oficina por ubicación geográfica o nombre de zona.
    /// </summary>
    /// <param name="query">Consulta con el nombre de la ubicación.</param>
    /// <returns>La oficina correspondiente a esa ubicación.</returns>
    Task<Office> Handle(GetOfficeByLocation query);
}
