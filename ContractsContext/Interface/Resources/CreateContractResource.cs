namespace workstation_backend.ContractsContext.Interface.Resources;

public record class CreateContractResource(    Guid OfficeId,
    Guid OwnerId,
    Guid RenterId,
    string Description,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal BaseAmount,
    decimal LateFee,
    decimal InterestRate);

