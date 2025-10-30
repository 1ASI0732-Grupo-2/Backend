using System;
using FluentValidation;
using workstation_backend.ContractsContext.Domain.Models.Commands;

namespace workstation_backend.ContractsContext.Domain.Models.Validators;

public class UpdateReceiptCommandValidator: AbstractValidator<UpdateReceiptCommand>
{
    public UpdateReceiptCommandValidator()
        {
            RuleFor(x => x.ContractId)
                .NotEmpty()
                .WithMessage("El ID del contrato es requerido.");

            RuleFor(x => x.Notes)
                .NotEmpty()
                .WithMessage("Las notas son requeridas.")
            .MaximumLength(1000)
            .WithMessage("Las notas no pueden exceder los 1000 caracteres.");
    }
}
