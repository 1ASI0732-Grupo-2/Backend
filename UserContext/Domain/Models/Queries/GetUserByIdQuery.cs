namespace workstation_backend.UserContext.Domain.Models.Queries;

/// <summary>
/// Consulta para obtener la información de un usuario por su identificador único.
/// </summary>
public record GetUserByIdQuery
{
    /// <summary>
    /// Inicializa una nueva instancia de la consulta con el ID del usuario.
    /// </summary>
    /// <param name="userId">Identificador único del usuario.</param>
    public GetUserByIdQuery(Guid userId)
    {
        UserId = userId;
    }

    /// <summary>
    /// Identificador del usuario que se desea consultar.
    /// </summary>
    public Guid UserId { get; init; }
}
