namespace workstation_backend.OfficesContext.Domain.Models.Queries;

/// <summary>
/// Consulta para obtener oficinas por ubicación.
/// </summary>
public record GetOfficeByLocation
{
    /// <summary>
    /// Inicializa una nueva instancia de la consulta con la ubicación especificada.
    /// </summary>
    /// <param name="officeLocation">Nombre o descripción de la ubicación (ej. Miraflores, San Isidro).</param>
    public GetOfficeByLocation(string officeLocation)
    {
        OfficeLocation = officeLocation;
    }

    /// <summary>
    /// Ubicación por la cual se desea filtrar las oficinas.
    /// </summary>
    public string OfficeLocation { get; init; }
}
