using System;
using FluentValidation;
using workstation_backend.ContractsContext.Domain.Models.Commands;

namespace workstation_backend.ContractsContext.Domain.Models.Validators;

public class AddClauseCommandValidator:AbstractValidator<AddClauseCommand>
{
 public AddClauseCommandValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty()
            .WithMessage("El ID del contrato es requerido.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("El nombre de la cláusula es requerido.")
            .MaximumLength(200)
            .WithMessage("El nombre de la cláusula no puede exceder los 200 caracteres.");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("El contenido de la cláusula es requerido.")
            .MaximumLength(2000)
            .WithMessage("El contenido de la cláusula no puede exceder los 2000 caracteres.");

        RuleFor(x => x.Order)
            .GreaterThan(0)
            .WithMessage("El orden debe ser mayor a cero.");
    }
}
