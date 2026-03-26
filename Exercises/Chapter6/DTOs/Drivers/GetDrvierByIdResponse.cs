namespace Chapter6.DTOs.Drivers;

public class GetDriverByIdResponse
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}