using System;
using Microsoft.EntityFrameworkCore;

using ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Domain;
using workstation_backend.Shared.Infrastructure.Persistence.Configuration;
using workstation_backend.Shared.Infrastructure.Persistence.Repositories;
using workstation_backend.ContractsContext.Domain.Models.Enums;

namespace workstation_backend.ContractsContext.Infrastructure;

public class ContractRepository(WorkstationContext context) : BaseRepository<Contract>(context), IContractRepository
{

    
    public async Task<Contract?> GetByIdAsync(Guid id)
    {
        return await context.Set<Contract>()
            .Include(c => c.Clauses)
            .Include(c => c.Signatures)
            .Include(c => c.Compensations)
            .Include(c => c.Receipt)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Contract?> GetActiveContractByOfficeIdAsync(Guid officeId)
    {
        return await context.Set<Contract>()
            .Include(c => c.Clauses)
            .Include(c => c.Signatures)
            .Include(c => c.Compensations)
            .Include(c => c.Receipt)
            .FirstOrDefaultAsync(c => c.OfficeId == officeId && c.Status == ContractStatus.Active);
    }

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

    public async Task AddAsync(Contract contract)
    {
        await context.Set<Contract>().AddAsync(contract);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
