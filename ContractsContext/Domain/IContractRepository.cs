using System;
using ContractsContext.Domain.Models.Entities;
using workstation_backend.Shared.Domain.Repositories;

namespace workstation_backend.ContractsContext.Domain;

public interface IContractRepository : IBaseRepository<Contract>
{
    Task<Contract?> GetByIdAsync(Guid id);
    Task<Contract?> GetActiveContractByOfficeIdAsync(Guid officeId);
    Task<List<Contract>> GetContractsByUserIdAsync(Guid userId);
    Task<List<Contract>> GetActiveContractsAsync();
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
