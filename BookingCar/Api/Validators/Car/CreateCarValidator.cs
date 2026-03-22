using FluentValidation;

public class CreateCarValidator : AbstractValidator<CreateCarRequest>
{
    private readonly ICarRepository _carRepository;

    public CreateCarValidator(ICarRepository carRepository)
    {
        _carRepository = carRepository;

        RuleFor(x => x.LicensePlate)
            .NotEmpty()
            .MaximumLength(20)
            .Must((licensePlate) =>
            {
                if (string.IsNullOrEmpty(licensePlate)) return true;
                var existingCar = _carRepository.GetByLicensePlateAsync(licensePlate).GetAwaiter().GetResult();
                return existingCar == null;
            })
            .WithMessage("License plate already exists");

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .LessThanOrEqualTo(60);

        RuleFor(x => x.Brand)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Seats)
            .NotNull()
            .NotEmpty().WithMessage("Seats are required")
            .Must(seats => seats != null && seats.Count > 0).WithMessage("At least one seat is required")
            .Must(seats => seats != null && seats.Select(s => s.Name).Distinct().Count() == seats.Count)
            .WithMessage("Seat names must be unique within the car request.");
    }
}