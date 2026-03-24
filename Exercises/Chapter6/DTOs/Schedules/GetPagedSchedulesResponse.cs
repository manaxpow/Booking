namespace Chapter6.DTOs.Schedules;

public class GetPagedSchedulesResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Count { get; set; }
    public IEnumerable<ScheduleSummaryResponse> Items { get; set; } = [];
}