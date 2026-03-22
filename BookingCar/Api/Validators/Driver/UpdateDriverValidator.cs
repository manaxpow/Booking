using FluentValidation;

public class UpdateDriverValidator : AbstractValidator<UpdateDriverRequest>
{
    private static readonly HashSet<string> ValidLicenses = new(StringComparer.OrdinalIgnoreCase)
    {
        "B",
        "C",
        "D"
    };

    public UpdateDriverValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Dob)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.UtcNow.Date).WithMessage("Date of birth must be in the past");

        RuleFor(x => x.License)
            .NotEmpty().WithMessage("License is required")
            .Must(license => ValidLicenses.Contains(license)).WithMessage("License must be B, C or D");
    }
}
