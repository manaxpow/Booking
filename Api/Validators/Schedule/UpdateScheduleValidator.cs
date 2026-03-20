using FluentValidation;
using Models.Dtos.Schedule;

namespace Api.Validators.Schedule;

public class UpdateScheduleValidator : AbstractValidator<UpdateScheduleRequest>
{
    public UpdateScheduleValidator()
    {
        

        When(x => x.CarId.HasValue, () =>
        {
            RuleFor(x => x.CarId!.Value)
                .GreaterThan(0)
                .WithMessage("Xe không tồn tại");
        });

        When(x => x.DriverId.HasValue, () =>
        {
            RuleFor(x => x.DriverId!.Value)
                .GreaterThan(0)
                .WithMessage("Tài xế không tồn tại");
        });

        When(x => x.FromDestinationId.HasValue && x.ToDestinationId.HasValue, () =>
        {
            RuleFor(x => x.FromDestinationId!.Value)
                .NotEqual(x => x.ToDestinationId!.Value)
                .WithMessage("Điểm đi và điểm đến không được trùng nhau");
        });

        When(x => x.StartTime.HasValue, () =>
        {
            RuleFor(x => x.StartTime!.Value)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Thời gian khởi hành phải sau thời gian hiện tại");
        });

        When(x => x.Price.HasValue, () =>
        {
            RuleFor(x => x.Price!.Value)
                .GreaterThan(0)
                .WithMessage("Giá vé phải lớn hơn 0");
        });

        When(x => !string.IsNullOrEmpty(x.Status), () =>
        {
            RuleFor(x => x.Status)
                .Must(status => new[] { "ACTIVE", "INACTIVE", "FINISH" }.Contains(status))
                .WithMessage("Trạng thái phải là ACTIVE, INACTIVE hoặc FINISH");
        });
    }
}