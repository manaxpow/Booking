public class DriverQueryParameters
{
    public string? Keyword { get; set; } // Search in name
    public string? License { get; set; } // B, C, D
    public DateTime? DobFrom { get; set; }
    public DateTime? DobTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
