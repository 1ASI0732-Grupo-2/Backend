namespace workstation_backend.OfficesContext.Domain.Models.Queries;

/// <summary>
/// Consulta para obtener una oficina por su identificador único.
/// </summary>
public record class GetOfficeByIdQuery
{
    /// <summary>
    /// Inicializa una nueva instancia de la consulta con el ID de la oficina.
    /// </summary>
    /// <param name="officeId">Identificador único de la oficina.</param>
    public GetOfficeByIdQuery(Guid officeId)
    {
        OfficeId = officeId;
    }

    /// <summary>
    /// Identificador de la oficina a consultar.
    /// </summary>
    public Guid OfficeId { get; init; }
}
