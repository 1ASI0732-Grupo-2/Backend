using System;
using Microsoft.EntityFrameworkCore;

using ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain;
using workstation_backend.ContractsContext.Domain.Models.Entities;
using workstation_backend.Shared.Infrastructure.Persistence.Configuration;
using workstation_backend.Shared.Infrastructure.Persistence.Repositories;
using workstation_backend.ContractsContext.Domain.Models.Enums;
namespace workstation_backend.ContractsContext.Infrastructure;

/// <summary>
/// Repositorio encargado de gestionar la persistencia de contratos en la base de datos.
/// Implementa operaciones de lectura y escritura utilizando Entity Framework Core.
/// Incluye consultas con carga ansiosa (Include) para recuperar información completa.
/// </summary>
public class ContractRepository(ContractContext context) 
    : BaseRepository<Contract>(context), IContractRepository
{
    /// <summary>
    /// Obtiene un contrato por su identificador único.
    /// Incluye cláusulas, firmas, compensaciones y recibo.
    /// </summary>
    /// <param name="id">Identificador del contrato.</param>
    /// <returns>El contrato correspondiente o null si no existe.</returns>
    public async Task<Contract?> GetByIdAsync(Guid id)
    {
        return await context.Set<Contract>()
            .Include(c => c.Clauses)
            .Include(c => c.Signatures)
            .Include(c => c.Compensations)
            .Include(c => c.Receipt)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>
    /// Obtiene el contrato activo asociado a una oficina específica.
    /// Devuelve null si no existe un contrato activo para dicha oficina.
    /// </summary>
    /// <param name="officeId">ID de la oficina.</param>
    /// <returns>El contrato activo o null.</returns>
    public async Task<Contract?> GetActiveContractByOfficeIdAsync(Guid officeId)
    {
        return await context.Set<Contract>()
            .Include(c => c.Clauses)
            .Include(c => c.Signatures)
            .Include(c => c.Compensations)
            .Include(c => c.Receipt)
            .FirstOrDefaultAsync(c => c.OfficeId == officeId && c.Status == ContractStatus.Active);
    }

    /// <summary>
    /// Obtiene todos los contratos donde el usuario participa como propietario o arrendatario.
    /// Ordena los resultados por fecha de creación en orden descendente.
    /// </summary>
    /// <param name="userId">ID del usuario.</param>
    /// <returns>Lista de contratos en los que participa el usuario.</returns>
    public async Task<List<Contract>> GetContractsByUserIdAsync(Guid userId)
    {
        return await context.Set<Contract>()
            .Include(c => c.Clauses)
            .Include(c => c.Signatures)
            .Include(c => c.Compensations)
            .Include(c => c.Receipt)
            .Where(c => c.OwnerId == userId || c.RenterId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene todos los contratos que actualmente se encuentran en estado activo.
    /// Ordena los resultados por fecha de activación en orden descendente.
    /// </summary>
    /// <returns>Lista de contratos activos.</returns>
    public async Task<List<Contract>> GetActiveContractsAsync()
    {
        return await context.Set<Contract>()
            .Include(c => c.Clauses)
            .Include(c => c.Signatures)
            .Include(c => c.Compensations)
            .Include(c => c.Receipt)
            .Where(c => c.Status == ContractStatus.Active)
            .OrderByDescending(c => c.ActivatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Agrega un nuevo contrato al contexto de persistencia.
    /// Se debe ejecutar <see cref="SaveChangesAsync"/> posteriormente para confirmar los cambios.
    /// </summary>
    /// <param name="contract">Contrato a agregar.</param>
    public async Task AddAsync(Contract contract)
    {
        await context.Set<Contract>().AddAsync(contract);
    }

    /// <summary>
    /// Agrega una firma explícitamente al contexto de persistencia.
    /// </summary>
    /// <param name="signature">Firma a agregar.</param>
    public async Task AddSignatureAsync(Signature signature)
    {
        await context.Set<Signature>().AddAsync(signature);
    }

    /// <summary>
    /// Guarda los cambios pendientes en la base de datos.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    /// <returns>Tarea asincrónica que confirma la operación.</returns>
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
