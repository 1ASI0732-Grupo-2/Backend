using System;
using FluentValidation;
using workstation_backend.ContractsContext.Domain.Models.Commands;

namespace workstation_backend.ContractsContext.Domain.Models.Validators;

public class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
{
    public CreateContractCommandValidator()
    {
        RuleFor(x => x.OfficeId)
            .NotEmpty().WithMessage("El ID de la oficina es requerido.");

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("El ID del propietario es requerido.");

        RuleFor(x => x.RenterId)
            .NotEmpty().WithMessage("El ID del arrendatario es requerido.")
            .NotEqual(x => x.OwnerId).WithMessage("El propietario y el arrendatario no pueden ser la misma persona.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es requerida.")
            .MaximumLength(500).WithMessage("La descripción no puede exceder los 500 caracteres.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("La fecha de inicio es requerida.")
            .Must(BeValidDate).WithMessage("La fecha de inicio no puede ser anterior a hoy.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("La fecha de fin es requerida.")
            .GreaterThan(x => x.StartDate).WithMessage("La fecha de fin debe ser posterior a la fecha de inicio.");

        RuleFor(x => x.BaseAmount)
            .GreaterThan(0).WithMessage("El monto base debe ser mayor a cero.");

        RuleFor(x => x.LateFee)
            .GreaterThanOrEqualTo(0).WithMessage("La penalidad por mora no puede ser negativa.");

        RuleFor(x => x.InterestRate)
            .GreaterThanOrEqualTo(0).WithMessage("La tasa de interés no puede ser negativa.")
            .LessThanOrEqualTo(100).WithMessage("La tasa de interés no puede exceder el 100%.");
    }

    private bool BeValidDate(DateOnly date)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        return date >= today;
    }
}
