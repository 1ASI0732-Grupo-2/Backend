using System;
using workstation_backend.ContractsContext.Domain.Models.Events;

namespace workstation_backend.ContractsContext.Domain.Services;

public interface IContractEventService
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;


}
