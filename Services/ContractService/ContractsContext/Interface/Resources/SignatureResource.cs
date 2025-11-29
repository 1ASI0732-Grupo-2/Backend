namespace workstation_backend.ContractsContext.Interface.Resources;

public record class SignatureResource(    Guid Id,
    Guid SignerId,
    DateTime SignedAt,
    string SignatureHash);

