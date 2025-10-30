namespace workstation_backend.ContractsContext.Domain.Models.Events;

public record ContractCreatedEvent(
    Guid ContractId,
    Guid OfficeId,
    Guid OwnerId,
    Guid RenterId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal BaseAmount,
    DateTime OccurredAt
);