namespace Chapter6.DTOs.Cars;

public class GetPagedCarsResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Count { get; set; }
    public IEnumerable<CarSummaryResponse> Items { get; set; } = [];
}