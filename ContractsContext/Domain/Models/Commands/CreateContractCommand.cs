using MediatR;

namespace workstation_backend.ContractsContext.Domain.Models.Commands;

public record CreateContractCommand(
    Guid OfficeId,
    Guid OwnerId,
    Guid RenterId,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    decimal BaseAmount,
    decimal LateFee,
    decimal InterestRate
) : IRequest<Guid>;