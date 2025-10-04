namespace workstation_backend.OfficesContext.Domain.Models.Queries;

/// <summary>
/// Consulta para obtener todas las calificaciones asociadas a una oficina.
/// </summary>
public class GetRatingsByOfficeQuery
{
    /// <summary>
    /// Identificador de la oficina cuyas calificaciones se desean obtener.
    /// </summary>
    public Guid OfficeId { get; set; }
}
