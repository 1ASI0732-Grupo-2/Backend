using System;
using workstation_backend.ContractsContext.Domain.Models.Entities;
using workstation_backend.ContractsContext.Interface.Resources;

namespace workstation_backend.ContractsContext.Interface.Transform;

public static class SignatureResourceAssembler
{
    public static SignatureResource ToResource(Signature signature)
    {
        return new SignatureResource(
            signature.Id,
            signature.SignerId,
            signature.SignedAt,
            signature.SignatureHash
        );
    }
}
