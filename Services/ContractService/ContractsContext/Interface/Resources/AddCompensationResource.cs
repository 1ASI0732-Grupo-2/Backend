namespace workstation_backend.ContractsContext.Interface.Resources;

public record class AddCompensationResource(    Guid IssuerId,
    Guid ReceiverId,
    decimal Amount,
    string Reason);
