using System;
using System.Text.Json;
using workstation_backend.ContractsContext.Domain.Services;

namespace workstation_backend.ContractsContext.Application.EventServices;

public class ContractEventService : IContractEventService
{
    private readonly ILogger<ContractEventService> _logger;

    public ContractEventService(ILogger<ContractEventService> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
    {
        var eventName = typeof(TEvent).Name;
        var eventData = JsonSerializer.Serialize(@event);

        _logger.LogInformation("[EVENT PUBLISHED] {EventName}: {EventData}", eventName, eventData);

        return Task.CompletedTask;    }
}
