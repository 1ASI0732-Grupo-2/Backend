namespace workstation_backend.OfficesContext.Infrastructure;

using Microsoft.EntityFrameworkCore;
using workstation_backend.OfficesContext.Domain;
using workstation_backend.OfficesContext.Domain.Models.Entities;
using workstation_backend.Shared.Infrastructure.Persistence.Configuration;
using workstation_backend.Shared.Infrastructure.Persistence.Repositories;



/// <summary>
/// Repositorio para operaciones relacionadas con la entidad <see cref="Rating"/>.
/// Implementa <see cref="IRatingRepository"/> directamente sin base genérica.
/// </summary>
public class RatingRepository : IRatingRepository
{
    private readonly WorkstationContext _context;

    /// <summary>
    /// Inicializa una nueva instancia del repositorio de calificaciones.
    /// </summary>
    /// <param name="context">Contexto de base de datos.</param>
    public RatingRepository(WorkstationContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todas las calificaciones asociadas a una oficina específica.
    /// </summary>
    /// <param name="officeId">Identificador único de la oficina.</param>
    /// <returns>Una colección de calificaciones.</returns>
    public async Task<IEnumerable<Rating>> GetByOfficeIdAsync(Guid officeId)
    {
        return await _context.Ratings
            .Where(r => r.OfficeId == officeId)
            .ToListAsync();
    }

    /// <summary>
    /// Agrega una nueva calificación al contexto.
    /// </summary>
    /// <param name="entity">La calificación a agregar.</param>
    public async Task AddAsync(Rating entity) => await _context.Ratings.AddAsync(entity);

    /// <summary>
    /// (No implementado) Buscar calificación por ID entero.
    /// </summary>
    public Task<Rating?> FindByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// (No implementado) Actualiza una calificación existente.
    /// </summary>
    public void Update(Rating entity)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Elimina una calificación del contexto.
    /// </summary>
    /// <param name="entity">Calificación a eliminar.</param>
    public void Remove(Rating entity) => _context.Ratings.Remove(entity);

    /// <summary>
    /// (No implementado) Lista todas las calificaciones.
    /// </summary>
    public Task<IEnumerable<Rating>> ListAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Obtiene una calificación por su ID (GUID).
    /// </summary>
    /// <param name="id">Identificador único de la calificación.</param>
    /// <returns>La calificación si existe, null si no.</returns>
    public Task<Rating?> GetByIdAsync(Guid id) => _context.Ratings.FirstOrDefaultAsync(r => r.Id == id);

    /// <summary>
    /// Retorna todas las calificaciones en memoria.
    /// </summary>
    public Task<IEnumerable<Rating>> GetAllAsync() =>
        Task.FromResult<IEnumerable<Rating>>(_context.Ratings.ToList());

    /// <summary>
    /// (No implementado) Buscar calificación por ID (GUID).
    /// </summary>
    public Task<Rating?> FindByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}

