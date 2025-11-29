namespace workstation_backend.ContractsContext.Interface.Resources;

public record class SignContractResource(Guid SignerId, string SignatureHash);

