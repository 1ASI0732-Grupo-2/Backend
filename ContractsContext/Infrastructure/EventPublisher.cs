using System;
using MediatR;
using workstation_backend.ContractsContext.Domain.Services;

namespace workstation_backend.ContractsContext.Infrastructure;

public class EventPublisher: IContractEventService
{
private readonly IMediator _mediator;

    public EventPublisher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
    {
        await _mediator.Publish(@event);
    }
}
