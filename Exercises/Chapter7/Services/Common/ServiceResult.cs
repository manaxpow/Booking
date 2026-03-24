namespace Chapter6.Services.Common;

public class ServiceResult
{
    public ServiceStatus Status { get; init; }
    public string? Message { get; init; }

    public static ServiceResult Success() => new() { Status = ServiceStatus.Success };

    public static ServiceResult NotFound(string message) =>
        new() { Status = ServiceStatus.NotFound, Message = message };

    public static ServiceResult BadRequest(string message) =>
        new() { Status = ServiceStatus.BadRequest, Message = message };

    public static ServiceResult Conflict(string message) =>
        new() { Status = ServiceStatus.Conflict, Message = message };
}

public class ServiceResult<T>
{
    public ServiceStatus Status { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }

    public static ServiceResult<T> Success(T data) =>
        new() { Status = ServiceStatus.Success, Data = data };

    public static ServiceResult<T> NotFound(string message) =>
        new() { Status = ServiceStatus.NotFound, Message = message };

    public static ServiceResult<T> BadRequest(string message) =>
        new() { Status = ServiceStatus.BadRequest, Message = message };

    public static ServiceResult<T> Conflict(string message) =>
        new() { Status = ServiceStatus.Conflict, Message = message };
}
