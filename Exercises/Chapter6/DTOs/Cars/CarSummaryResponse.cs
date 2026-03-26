namespace Chapter6.DTOs.Cars;

public class CarSummaryResponse
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int SeatCount { get; set; }
    public string? Color { get; set; }
    public int? ManufactureYear { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}