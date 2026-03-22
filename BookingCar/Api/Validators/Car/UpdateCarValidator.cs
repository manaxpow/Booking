using FluentValidation;

public class UpdateCarValidator : AbstractValidator<UpdateCarRequest>
{
    private readonly ICarRepository _carRepository;

    public UpdateCarValidator(ICarRepository carRepository)
    {
        _carRepository = carRepository;

        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.LicensePlate)
            .NotEmpty()
            .MaximumLength(20)
            .Must((request, licensePlate) =>
            {
                if (string.IsNullOrEmpty(licensePlate)) return true;
                var existingCar = _carRepository.GetByLicensePlateAsync(licensePlate).GetAwaiter().GetResult();
                // Valid if the license plate belongs to the same car or does not exist
                return existingCar == null || existingCar.Id == request.Id;
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
            .Must(seats => seats != null && seats.Select(s => s.Name).Distinct().Count() == seats.Count)
            .WithMessage("Seat names must be unique within the car request.");
    }
}
