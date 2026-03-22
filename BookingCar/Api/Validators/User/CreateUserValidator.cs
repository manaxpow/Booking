using FluentValidation;

public class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    private static readonly HashSet<string> ValidRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "ADMIN",
        "USER"
    };

    public CreateUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required")
            .Matches(@"^\d{10}$").WithMessage("Phone must contain 10 digits");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");

        RuleFor(x => x.Cccd)
            .NotEmpty().WithMessage("CCCD is required")
            .Matches(@"^\d{12}$").WithMessage("CCCD must contain 12 digits");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(role => ValidRoles.Contains(role)).WithMessage("Role must be ADMIN or USER");
    }
}
