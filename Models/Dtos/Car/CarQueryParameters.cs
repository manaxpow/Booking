namespace VehicleBooking.Models.DTOs.Car;

public class CarQueryParameters
{
    public string? Keyword { get; set; } // Tìm kiếm theo LicensePlate
    public string? Brand { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
