public static class CacheKeyFactory
{
    public const string AppPrefix = "BookingCar_";
    public const string OutputCachePrefix = "Booking_Output_";
    public const string DriverTag = "tag_drivers";
    public const string CarTag = "tag_cars";
    public const string DestinationTag = "tag_destinations";

    public static string GetScheduleKey(int id) => $"schedule:{id}";

    public static string GetSeatHashKey(int scheduleId) => $"schedule:{scheduleId}:seats";

    public static string GetCarKey(int carId) => $"car:{carId}";
    public static string GetCarDetailKey(int carId) => $"car:{carId}:detail";
    public static string GetDriverKey(int driverId) => $"driver:{driverId}";

    public static string GetAvailableSeatsCounterKey(int scheduleId) => $"schedule:{scheduleId}:count";

    public static string GetDestinationKey(int destinationId) => $"destination:{destinationId}";
}