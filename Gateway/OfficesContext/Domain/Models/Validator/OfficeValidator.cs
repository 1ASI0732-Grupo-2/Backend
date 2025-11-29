using System;
using FluentValidation;
using workstation_backend.OfficesContext.Domain.Models.Commands;
using workstation_backend.OfficesContext.Domain.Models.Entities;

namespace workstation_backend.OfficesContext.Domain.Models.Validator;

public class OfficeValidator : AbstractValidator<CreateOfficeCommand>
{
    public OfficeValidator()
    {
        RuleFor(office => office.Location)
            .NotEmpty().WithMessage("The office location must be listed.")
            .MaximumLength(200).WithMessage("Location can't exceed 200 characters.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be more than 0.")
            .LessThanOrEqualTo(1000).WithMessage("Capacity can't be more than 1000.");

        RuleFor(x => x.CostPerDay)
            .GreaterThan(0).WithMessage("Cost per day must be more than 0.")
            .LessThan(100000).WithMessage("Cost per day is too much!");

        RuleFor(x => x.Services)
            .NotNull().WithMessage("Define a list of services.")
            .Must(x => x.Count > 0).WithMessage("Include at least one service.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MaximumLength(500).WithMessage("La descripción no debe exceder los 500 caracteres.");

        RuleFor(x => x.ImageUrl)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("La URL de la imagen no puede estar vacía.")
            .Must(BeAValidImageUrl)
                .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
                .WithMessage("La URL debe ser válida y apuntar a una imagen (.jpg, .jpeg, .png o .webp).")
            .MaximumLength(300).WithMessage("La URL de la imagen no debe exceder los 300 caracteres.");

        RuleForEach(x => x.Services).SetValidator(new OfficeServiceValidator());

        RuleFor(x => x)
            .Custom((command, context) =>
            {
                var serviceCount = command.Services?.Count ?? 0;

                if (serviceCount <= 2 && command.CostPerDay > 54)
                {
                    context.AddFailure("CostPerDay", $"The daily cost cannot exceed 54 when there are {serviceCount} or fewer services.");
                }

                if (serviceCount > 4 && command.CostPerDay > 80)
                {
                    context.AddFailure("CostPerDay", $"The daily cost cannot exceed 80 when there are more than 4 services.");
                }
            });
    }
        private bool BeAValidImageUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult))
            return false;

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        return allowedExtensions.Any(ext => uriResult.AbsolutePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }
}
