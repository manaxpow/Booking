using FluentValidation;
using Models.Dtos.Schedule;

namespace Api.Validators.Schedule;

public class CreateScheduleValidator : AbstractValidator<CreateScheduleRequest>
{
    public CreateScheduleValidator()
    {
        RuleFor(x => x.CarId)
            .GreaterThan(0)
            .WithMessage("Xe không tồn tại");

        RuleFor(x => x.DriverId)
            .GreaterThan(0)
            .WithMessage("Tài xế không tồn tại");

        RuleFor(x => x.FromDestinationId)
            .GreaterThan(0)
            .WithMessage("Điểm đi không tồn tại");

        RuleFor(x => x.ToDestinationId)
            .GreaterThan(0)
            .WithMessage("Điểm đến không tồn tại");

        RuleFor(x => x.FromDestinationId)
            .NotEqual(x => x.ToDestinationId)
            .WithMessage("Điểm đi và điểm đến không được trùng nhau");

        RuleFor(x => x.StartTime)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Thời gian khởi hành phải sau thời gian hiện tại");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Giá vé phải lớn hơn 0");
        
        RuleFor(x => x.ExpectedDuration)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Thời lượng dự kiến phải lớn hơn 0");

        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(status => new[] { "ACTIVE", "INACTIVE", "FINISH" }.Contains(status))
            .WithMessage("Trạng thái phải là ACTIVE, INACTIVE hoặc FINISH");
    }
}