namespace Chapter6.DTOs.Destinations;

public class GetDestinationByIdResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsActive { get; set; }
}
