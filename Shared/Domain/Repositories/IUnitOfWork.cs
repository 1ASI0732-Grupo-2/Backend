using System;

namespace workstation_backend.Shared.Domain.Repositories;

public interface IUnitOfWork
{
   Task CompleteAsync();
}
