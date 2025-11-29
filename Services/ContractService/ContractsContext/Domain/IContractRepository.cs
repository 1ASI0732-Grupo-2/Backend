using System;
using ContractsContext.Domain.Models.Entities;
using workstation_backend.Shared.Domain.Repositories;
using workstation_backend.ContractsContext.Domain.Models.Entities;

namespace workstation_backend.ContractsContext.Domain;

public interface IContractRepository : IBaseRepository<Contract>
{
    Task<Contract?> GetByIdAsync(Guid id);
    Task<Contract?> GetActiveContractByOfficeIdAsync(Guid officeId);
    Task<List<Contract>> GetContractsByUserIdAsync(Guid userId);
    Task<List<Contract>> GetActiveContractsAsync();
    Task AddSignatureAsync(Signature signature);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
