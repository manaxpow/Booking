using System.Text.Json.Serialization;

namespace VehicleBooking.Models.DTOs;

public class SuccessResponse<T>
{
    public int StatusCode { get; set; } =200;
    public string Message { get; set; } = string.Empty;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; set; }
}
