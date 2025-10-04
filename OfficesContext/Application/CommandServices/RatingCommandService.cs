using workstation_backend.OfficesContext.Domain;
using workstation_backend.OfficesContext.Domain.Models.Commands;
using workstation_backend.Shared.Domain.Repositories;

namespace workstation_backend.OfficesContext.Application.CommandServices;

/// <summary>
/// Servicio encargado de manejar los comandos relacionados con calificaciones (<see cref="Rating"/>).
/// </summary>
public class RatingCommandService
{
    private readonly IRatingRepository _ratingRepository;

    private readonly IOfficeRepository _officeRepository;

    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de comandos de calificaciones.
    /// </summary>
    /// <param name="ratingRepository">Repositorio de calificaciones.</param>
    /// <param name="unitOfWork">Unidad de trabajo para confirmar cambios.</param>
    public RatingCommandService(IRatingRepository ratingRepository, IOfficeRepository officeRepository, IUnitOfWork unitOfWork)
    {
        _ratingRepository = ratingRepository;
        _officeRepository = officeRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
/// Crea una nueva calificación asociada a una oficina existente.
/// </summary>
/// <param name="command">Comando con los datos de la calificación.</param>
/// <returns>La entidad <see cref="Rating"/> creada.</returns>
/// <exception cref="ArgumentNullException">Si el comando es nulo.</exception>
/// <exception cref="ArgumentException">Si el comentario excede los 500 caracteres.</exception>
/// <exception cref="ArgumentOutOfRangeException">Si el puntaje está fuera del rango 0-5.</exception>
/// <exception cref="KeyNotFoundException">Si no existe la oficina con el ID proporcionado.</exception>

public async Task<Rating> CreateRatingAsync(CreateRatingCommand command)
{
    const int MaxCommentLength = 500;
    const int MinScore = 0;
    const int MaxScore = 5;
    ArgumentNullException.ThrowIfNull(command);

    if (!string.IsNullOrWhiteSpace(command.Comment) && command.Comment.Length > MaxCommentLength)
    {
        throw new ArgumentException("Comment cannot exceed 500 characters.");
    }

    if (command.Score < MinScore || command.Score > MaxScore)
    {
        throw new ArgumentOutOfRangeException(nameof(command.Score), "Score must be between 0 and 5.");
    }

    var officeExists = await _officeRepository.FindByIdAsync(command.OfficeId);
    if (officeExists is null)
    {
        throw new KeyNotFoundException($"Office with ID {command.OfficeId} does not exist.");
    }

    var rating = new Rating
    {
        Id = Guid.NewGuid(),
        Score = command.Score,
        Comment = command.Comment,
        OfficeId = command.OfficeId
    };

    await _ratingRepository.AddAsync(rating);
    await _unitOfWork.CompleteAsync();

    return rating;
}


}
