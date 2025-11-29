using System;
using FluentValidation;
using workstation_backend.ContractsContext.Domain.Models.Commands;

namespace workstation_backend.ContractsContext.Domain.Models.Validators;

public class SignContractCommandValidator: AbstractValidator<SignContractCommand>
{
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
            .MinimumLength(8)
            .WithMessage("El hash de la firma debe tener al menos 32 caracteres.")
            .MaximumLength(256)
            .WithMessage("El hash de la firma no puede exceder los 256 caracteres.");
    }
}
