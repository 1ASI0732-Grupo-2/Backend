namespace workstation_backend.ContractsContext.Interface.Resources;

public record class CreateContractResource(    Guid OfficeId,
    Guid OwnerId,
    Guid RenterId,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    decimal BaseAmount,
    decimal LateFee,
    decimal InterestRate);

