using FluentValidation;

public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
{
    private static readonly HashSet<string> ValidRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "ADMIN",
        "USER"
    };

    public UpdateUserValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.Phone)
            .Matches(@"^\d{10}$").WithMessage("Phone must contain 10 digits")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Password)
            .MinimumLength(6).WithMessage("Password must be at least 6 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Password));

        RuleFor(x => x.Cccd)
            .Matches(@"^\d{12}$").WithMessage("CCCD must contain 12 digits")
            .When(x => !string.IsNullOrWhiteSpace(x.Cccd));

        RuleFor(x => x.Role)
            .Must(role => role == null || ValidRoles.Contains(role)).WithMessage("Role must be ADMIN or USER");
    }
}
