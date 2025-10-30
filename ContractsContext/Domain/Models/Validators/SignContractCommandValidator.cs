using System;
using FluentValidation;
using workstation_backend.ContractsContext.Domain.Models.Commands;
using workstation_backend.ContractsContext.Domain.Models.Enums;

namespace workstation_backend.ContractsContext.Domain.Models.Validators;

public class SignContractCommandValidator: AbstractValidator<SignContractCommand>
{
        private readonly IContractRepository _contractRepository;
    public SignContractCommandValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty()
            .WithMessage("El ID del contrato es requerido.");

        RuleFor(x => x.SignerId)
            .NotEmpty()
            .WithMessage("El ID del firmante es requerido.");

        RuleFor(x => x.SignatureHash)
            .NotEmpty()
            .WithMessage("El hash de la firma es requerido.")
            .MinimumLength(32)
            .WithMessage("El hash de la firma debe tener al menos 32 caracteres.")
            .MaximumLength(256)
            .WithMessage("El hash de la firma no puede exceder los 256 caracteres.");

        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
                await BeAValidSigner(command.ContractId, command.SignerId))
            .WithMessage("El firmante debe ser el propietario o el arrendatario del contrato.")
            .WithName("SignerId");

        RuleFor(x => x)
            .MustAsync(async (command, cancellation) =>
                await NotAlreadySigned(command.ContractId, command.SignerId))
            .WithMessage("Este usuario ya ha firmado el contrato.")
            .WithName("SignerId");

        RuleFor(x => x.ContractId)
            .MustAsync(async (contractId, cancellation) =>
                await ContractCanBeSigned(contractId))
            .WithMessage("El contrato no se encuentra en un estado válido para ser firmado.");
    }
    private async Task<bool> ContractExists(Guid contractId)
    {
        var contract = await _contractRepository.GetByIdAsync(contractId);
        return contract != null;
    }

    private async Task<bool> BeAValidSigner(Guid contractId, Guid signerId)
    {
        var contract = await _contractRepository.GetByIdAsync(contractId);
        
        if (contract == null)
            return false;
        
        return contract.OwnerId == signerId || contract.RenterId == signerId;
    }

    private async Task<bool> NotAlreadySigned(Guid contractId, Guid signerId)
    {
        var contract = await _contractRepository.GetByIdAsync(contractId);
        
        if (contract == null)
            return false;
        
        return !contract.Signatures.Any(s => s.SignerId == signerId);
    }

    private async Task<bool> ContractCanBeSigned(Guid contractId)
    {
        var contract = await _contractRepository.GetByIdAsync(contractId);
        
        if (contract == null)
            return false;
        
        return contract.Status == ContractStatus.Draft || 
               contract.Status == ContractStatus.PendingSignatures;
    }
}
