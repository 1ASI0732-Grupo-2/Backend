using System;
using FluentValidation;
using workstation_backend.ContractsContext.Domain.Models.Commands;

namespace workstation_backend.ContractsContext.Domain.Models.Validators;

public class FinishContractCommandValidator: AbstractValidator<FinishContractCommand>
{
 public FinishContractCommandValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty()
            .WithMessage("El ID del contrato es requerido.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("La razón de finalización es requerida.")
            .MaximumLength(500)
            .WithMessage("La razón no puede exceder los 500 caracteres.");
    }
}
