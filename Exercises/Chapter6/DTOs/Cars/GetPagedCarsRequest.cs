namespace Chapter6.DTOs.Cars;

public class GetPagedCarsRequest
{
    public string? Keyword { get; set; }
    public string? Brand { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}