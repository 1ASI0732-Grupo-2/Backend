using System;
using workstation_backend.OfficesContext.Domain;
using workstation_backend.OfficesContext.Domain.Models.Entities;
using workstation_backend.OfficesContext.Domain.Models.Queries;
using workstation_backend.OfficesContext.Domain.Services;

namespace workstation_backend.OfficesContext.Application.QueryServices;

/// <summary>
/// Servicio encargado de manejar consultas relacionadas con oficinas.
/// Implementa operaciones para listar, buscar por ID y buscar por ubicación.
/// </summary>
public class OfficeQueryService : IOfficeQueryService
{
    // Repositorio de oficinas usado para acceder a los datos.
    private readonly IOfficeRepository _officeRepository;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de consultas de oficinas.
    /// </summary>
    /// <param name="officeRepository">Repositorio de oficinas.</param>
    /// <exception cref="ArgumentNullException">Si el repositorio es nulo.</exception>
    public OfficeQueryService(IOfficeRepository officeRepository)
    {
        _officeRepository = officeRepository ?? throw new ArgumentNullException(nameof(officeRepository));
    }

    /// <summary>
    /// Maneja la consulta para obtener todas las oficinas activas.
    /// </summary>
    /// <param name="query">Consulta que representa la solicitud (puede estar vacía).</param>
    /// <returns>Una colección de oficinas activas.</returns>
    public async Task<IEnumerable<Office>> Handle(GetAllOfficesQuery query)
    {
        var offices = await _officeRepository.ListAsync();
        return offices?.Where(office => office.IsActive) ?? Enumerable.Empty<Office>();
    }

    /// <summary>
    /// Maneja la consulta para obtener una oficina por su identificador.
    /// </summary>
    /// <param name="query">Consulta con el ID de la oficina.</param>
    /// <returns>La oficina encontrada si está activa; null si no existe o está inactiva.</returns>
    /// <exception cref="ArgumentNullException">Si el query es nulo.</exception>
    public async Task<Office?> Handle(GetOfficeByIdQuery query)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));

        var office = await _officeRepository.FindByIdAsync(query.OfficeId);
        return office?.IsActive == true ? office : null;
    }

    /// <summary>
    /// Maneja la consulta para obtener una oficina por su ubicación.
    /// </summary>
    /// <param name="query">Consulta con el nombre o zona de la ubicación.</param>
    /// <returns>La oficina encontrada si está activa; null si no existe o está inactiva.</returns>
    /// <exception cref="ArgumentNullException">Si el query es nulo.</exception>
    public async Task<Office?> Handle(GetOfficeByLocation query)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));

        var office = await _officeRepository.GetByLocationAsync(query.OfficeLocation);
        return office?.IsActive == true ? office : null;
    }
}


