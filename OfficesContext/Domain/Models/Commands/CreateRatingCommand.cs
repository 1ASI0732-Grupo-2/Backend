namespace workstation_backend.OfficesContext.Domain.Models.Commands;

/// <summary>
/// Comando para crear una calificación de una oficina.
/// </summary>
public class CreateRatingCommand
{
    /// <summary>
    /// Puntaje dado por el usuario (ej. 1 a 5).
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Comentario opcional del usuario.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Identificador de la oficina que se califica.
    /// </summary>
    public Guid OfficeId { get; set; }
}
