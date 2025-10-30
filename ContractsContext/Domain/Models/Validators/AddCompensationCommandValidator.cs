using System;
using FluentValidation;
using workstation_backend.ContractsContext.Domain.Models.Commands;

namespace workstation_backend.ContractsContext.Domain.Models.Validators;

public class AddCompensationCommandValidator: AbstractValidator<AddCompensationCommand>
{
public AddCompensationCommandValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty()
            .WithMessage("El ID del contrato es requerido.");

        RuleFor(x => x.IssuerId)
            .NotEmpty()
            .WithMessage("El ID del emisor es requerido.");

        RuleFor(x => x.ReceiverId)
            .NotEmpty()
            .WithMessage("El ID del receptor es requerido.");

        RuleFor(x => x.ReceiverId)
            .NotEqual(x => x.IssuerId)
            .WithMessage("El emisor y el receptor no pueden ser la misma persona.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("El monto de la compensación debe ser mayor a cero.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("La razón de la compensación es requerida.")
            .MaximumLength(500)
            .WithMessage("La razón no puede exceder los 500 caracteres.");
    }
}
