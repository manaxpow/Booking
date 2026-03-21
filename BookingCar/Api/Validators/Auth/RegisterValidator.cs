using FluentValidation;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Số điện thoại không được để trống")
            .Matches(@"^\d{10}$").WithMessage("Số điện thoại phải bao gồm 10 chữ số");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email là bắt buộc")
            .EmailAddress().WithMessage("Định dạng Email không hợp lệ");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Vui lòng nhập họ tên")
            .MaximumLength(100).WithMessage("Tên quá dài");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu không được để trống")
            .MinimumLength(6).WithMessage("Mật khẩu phải từ 6 ký tự trở lên");

        RuleFor(x => x.Cccd)
            .NotEmpty().WithMessage("CCCD là bắt buộc")
            .Matches(@"^\d{12}$").WithMessage("CCCD phải có 12 chữ số");
    }
}