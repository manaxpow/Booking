public class CarService : ICarService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISeatService _seatService;

    public CarService(IUnitOfWork unitOfWork, ISeatService seatService)
    {
        _unitOfWork = unitOfWork;
        _seatService = seatService;
    }

    public async Task CreateCarAsync(CreateCarRequest request)
    {
        var car = new Car
        {
            LicensePlate = request.LicensePlate,
            Capacity = request.Capacity,
            Brand = request.Brand,
            Seats = new List<Seat>()
        };

        var seatNames = new HashSet<string>();

        foreach (var seatRequest in request.Seats)
        {
            if (!seatNames.Add(seatRequest.Name))
            {
                throw new Exception($"Seat name '{seatRequest.Name}' is duplicated in the request.");
            }

            var seat = new Seat
            {
                Name = seatRequest.Name,
                Car = car
            };
            car.Seats.Add(seat);
        }

        await _unitOfWork.Cars.AddAsync(car);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateCarAsync(UpdateCarRequest request)
    {
        var car = await _unitOfWork.Cars.GetCarWithSeatsByIdAsync(request.Id);
        if (car == null) throw new Exception("Car not found.");

        if (car.LicensePlate != request.LicensePlate)
        {
            var existingCar = await _unitOfWork.Cars.GetByLicensePlateAsync(request.LicensePlate);
            if (existingCar != null) throw new Exception("License plate already exists.");
        }

        car.LicensePlate = request.LicensePlate;
        car.Capacity = request.Capacity;
        car.Brand = request.Brand;
        car.UpdateAt = DateTime.UtcNow;

        var currentSeatNames = new HashSet<string>();

        if (request.Seats != null)
        {
            // Remove seats that are not in the new request
            var incomingSeatIds = request.Seats.Where(s => s.Id != null).Select(s => s.Id.GetValueOrDefault()).ToList();
            var seatsToRemove = car.Seats.Where(s => !incomingSeatIds.Contains(s.Id)).ToList();
            foreach (var seat in seatsToRemove)
            {
                _unitOfWork.Seats.Remove(seat);
                car.Seats.Remove(seat);
            }

            // Update existing or add new seats
            foreach (var seatReq in request.Seats)
            {
                if (!currentSeatNames.Add(seatReq.Name))
                {
                    throw new Exception($"Seat name '{seatReq.Name}' is duplicated in the request.");
                }

                if (seatReq.Id.HasValue)
                {
                    var existingSeat = car.Seats.FirstOrDefault(s => s.Id == seatReq.Id.Value);
                    if (existingSeat != null)
                    {
                        existingSeat.Name = seatReq.Name;
                        existingSeat.UpdateAt = DateTime.UtcNow;
                    }
                    else
                    {
                        // Fallback if ID was passed but not found, act as new or throw
                        throw new Exception($"Seat with ID {seatReq.Id.Value} not found in this car.");
                    }
                }
                else
                {
                    // Add new seat
                    if (!await _seatService.IsSeatNameUniqueAsync(seatReq.Name, car.Id))
                    {
                        throw new Exception($"Seat name '{seatReq.Name}' already exists in this car.");
                    }
                    var newSeat = new Seat { Name = seatReq.Name, Car = car };
                    car.Seats.Add(newSeat);
                }
            }
        }

        _unitOfWork.Cars.Update(car);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteCarAsync(int id)
    {
        var car = await _unitOfWork.Cars.GetByIdAsync(id);
        if (car == null) throw new Exception("Car not found.");

        _unitOfWork.Cars.Remove(car);
        await _unitOfWork.CompleteAsync();
    }
}