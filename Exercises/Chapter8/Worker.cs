using Chapter8;
using Microsoft.EntityFrameworkCore;

public class SeatCleanupWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    public SeatCleanupWorker(IServiceProvider services) => _services = services;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var expired = await db.Seats
                .Where(s => s.Status == SeatStatus.Hold && s.ExpiredTime < DateTime.UtcNow)
                .ToListAsync();

            if (expired.Any())
            {
                expired.ForEach(s => { s.Status = SeatStatus.Available; s.ExpiredTime = null; });
                await db.SaveChangesAsync();
                Console.WriteLine($"[CRON] Đã dọn dẹp {expired.Count} ghế quá hạn.");
            }
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Quét mỗi 30 giây
        }
    }
}