using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ExpiredBookingWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ExpiredBookingWorker> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // Chạy mỗi 1 phút

    public ExpiredBookingWorker(IServiceScopeFactory scopeFactory, ILogger<ExpiredBookingWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Expired Booking Worker đang khởi tạo...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var expiredPayments = await unitOfWork.Payments.GetExpiredPendingPaymentsAsync();

                    if (expiredPayments.Any())
                    {
                        _logger.LogInformation($"Tìm thấy {expiredPayments.Count()} giao dịch hết hạn.");

                        foreach (var payment in expiredPayments)
                        {
                            // Update status
                            payment.Status = PaymentStatus.EXPRIED;

                            var ticket = payment.Ticket;
                            if (ticket != null)
                            {
                                ticket.Status = TicketStatus.CANCELLED;
                                ticket.UpdateAt = DateTime.UtcNow;

                                foreach (var detail in ticket.TicketDetails)
                                {
                                    if (detail.SeatBooking != null)
                                    {
                                        detail.SeatBooking.IsHold = false;
                                        _logger.LogInformation($"Đã nhả ghế ID: {detail.SeatBookingId}");
                                    }
                                }
                            }
                        }

                        await unitOfWork.CompleteAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chạy dọn dẹp payment hết hạn.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}