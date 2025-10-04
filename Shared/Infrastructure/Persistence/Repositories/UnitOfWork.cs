using System;
using workstation_backend.Shared.Domain.Repositories;
using workstation_backend.Shared.Infrastructure.Persistence.Configuration;

namespace workstation_backend.Shared.Infrastructure.Persistence.Repositories;

public class UnitOfWork(WorkstationContext context) : IUnitOfWork
{
    public async Task CompleteAsync()
    {
        await context.SaveChangesAsync();
    }
}
