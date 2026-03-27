using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Chapter10.DataAccess.Data;
using Chapter10.DataAccess.Entities;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;

    public BookingController(AppDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet("seats/{scheduleId}")]
    public async Task<IActionResult> GetSeats(int scheduleId)
    {
        string cacheKey = $"seats_{scheduleId}";
        // Kiểm tra Redis
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            // Cache Hit
            Console.WriteLine("Cache hit");
            return Ok(JsonConvert.DeserializeObject<List<SeatBooking>>(cachedData));
        }

        // Cache Miss
        var seats = await _context.SeatBookings
            .Where(s => s.ScheduleId == scheduleId)
            .AsNoTracking()
            .ToListAsync();

        if (!seats.Any()) return NotFound("Không tìm thấy lịch trình này.");

        // Set cache
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };
        await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(seats), options);
        Console.WriteLine("Cache miss");
        return Ok(seats);
    }

    [HttpPost("book")]
    public async Task<IActionResult> BookSeats([FromBody] List<int> seatIds)
    {
        if (seatIds == null || !seatIds.Any()) return BadRequest("Danh sách ghế trống.");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var seats = await _context.SeatBookings
                .Where(s => seatIds.Contains(s.Id))
                .ToListAsync();

            if (seats.Any(s => s.IsBooking || s.IsHold))
                return BadRequest("Một số ghế đã được đặt hoặc đang bị giữ.");

            int scheduleId = seats.First().ScheduleId;

            foreach (var seat in seats)
            {
                seat.IsBooking = true;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Invalidate cache
            await _cache.RemoveAsync($"seats_{scheduleId}");
            Console.WriteLine("Invalidate cache");
            return Ok(new { message = "Đặt vé thành công", bookedIds = seatIds });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
        }
    }

    [HttpDelete("cancel/{seatId}")]
    public async Task<IActionResult> CancelSeat(int seatId)
    {
        var seat = await _context.SeatBookings.FindAsync(seatId);
        if (seat == null) return NotFound();

        seat.IsBooking = false;
        seat.IsHold = false;

        await _context.SaveChangesAsync();

        // Invalidate cache
        await _cache.RemoveAsync($"seats_{seat.ScheduleId}");

        return Ok("Đã hủy vé thành công.");
    }
}