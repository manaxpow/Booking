using Microsoft.Extensions.Caching.Distributed;

public static class CacheHelper
{
    public static DistributedCacheEntryOptions DefaultOptions => new()
    {
        // TTL (Time To Live): Dữ liệu tự hủy sau 5 phút nếu không có tác động
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
        // Sliding Expiration: Nếu có người truy cập, thời gian hết hạn sẽ được gia hạn thêm 2 phút
        SlidingExpiration = TimeSpan.FromMinutes(2)
    };
}