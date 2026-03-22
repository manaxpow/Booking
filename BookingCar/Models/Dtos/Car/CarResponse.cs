namespace VehicleBooking.Models.DTOs.Car;

public record CarResponse(int Id, string LicensePlate, int Capacity, string Brand);

public record CarDetailResponse(int Id, string LicensePlate, int Capacity, string Brand, List<SeatResponse> Seats);

public record SeatResponse(int Id, string Name);
