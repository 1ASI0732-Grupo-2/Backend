using workstation_backend.OfficesContext.Domain.Models.Entities;
using workstation_backend.Shared.Domain.Model;

/// <summary>
/// Representa una calificación dada por un usuario a una oficina.
/// </summary>
public class Rating : BaseEntity
{
    /// <summary>
    /// Puntaje asignado a la oficina, típicamente entre 1 y 5.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Comentario opcional que acompaña la calificación.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Identificador de la oficina a la que pertenece esta calificación.
    /// </summary>
    public Guid OfficeId { get; set; }

    /// <summary>
    /// Oficina asociada a esta calificación.
    /// </summary>
    public Office Office { get; set; }

    /// <summary>
    /// Fecha y hora en la que se creó la calificación (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
