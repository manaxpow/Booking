using Microsoft.EntityFrameworkCore;
using VehicleBooking.Models.DTOs.Common;
using Models.Dtos.Ticket;
using DataAccess.Data;

namespace Services.Ticket;

public class TicketService : ITicketService
{
    private readonly IUnitOfWork _unitOfWork;

    public TicketService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<TicketResponse>> GetTicketsByUserIdAsync(int userId, int page = 1, int pageSize = 10)
    {
        var tickets = await _unitOfWork.Tickets.GetByUserIdAsync(userId);
        Console.WriteLine(tickets.Count);
        var ticketResponses = new List<TicketResponse>();

        foreach (var ticket in tickets)
        {
            var details = await _unitOfWork.TicketDetails.GetTicketDetailByTicketIdAsync(ticket.Id);
            Console.WriteLine(details.Count());
            foreach (var detail in details)
            {
                var seatBooking = detail.SeatBooking;
                var schedule = seatBooking.Schedule;
                var car = schedule.Car;
                var driver = schedule.Driver;
                var seat = seatBooking.Seat;
                var destinationFrom = schedule.FromDestination;
                var destinationTo = schedule.ToDestination;

                ticketResponses.Add(new TicketResponse(
                    ticket.Id,
                    ticket.UserId,
                    seat.Id,
                    schedule.Id,
                    (int)schedule.Price,
                    destinationFrom?.Province ?? "",
                    destinationTo?.Province ?? "",
                    seat.Name,
                    car.Brand,
                    driver.Name,
                    schedule.StartTime,
                    schedule.ExpectedDuration,
                    ticket.CreateAt
                ));
            }
        }

        // Apply pagination
        var totalCount = ticketResponses.Count;
        var pagedItems = ticketResponses
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<TicketResponse>
        {
            Items = pagedItems,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<TicketDetailResponse?> GetTicketByIdAsync(int ticketId)
    {
        var ticket = await _unitOfWork.Tickets.GetTicketWithUserByIdAsync(ticketId);
        if (ticket == null) return null;

        var details = await _unitOfWork.TicketDetails.GetTicketDetailByTicketIdAsync(ticketId);
        if (!details.Any()) return null;

        // Return first ticket detail as main ticket response
        var detail = details.First();
        var seatBooking = detail.SeatBooking;
        var schedule = seatBooking.Schedule;
        var car = schedule.Car;
        var driver = schedule.Driver;
        var seat = seatBooking.Seat;
        var destinationFrom = schedule.FromDestination;
        var destinationTo = schedule.ToDestination;

        return new TicketDetailResponse(
            ticket.Id,
            ticket.UserId,
            seat.Id,
            schedule.Id,
            (int)schedule.Price,
            destinationFrom?.Province ?? "",
            destinationTo?.Province ?? "",
            details.Select(d => d.SeatBooking.Seat.Name).ToList(),
            car.Brand,
            driver.Name,
            schedule.StartTime,
            schedule.ExpectedDuration,
            ticket.CreateAt
        );
    }

    public async Task<List<TicketResponse>> GetTicketDetailsAsync(int ticketId)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
        if (ticket == null) return new List<TicketResponse>();

        var details = await _unitOfWork.TicketDetails.GetTicketDetailByTicketIdAsync(ticketId);
        var ticketResponses = new List<TicketResponse>();

        foreach (var detail in details)
        {
            var seatBooking = detail.SeatBooking;
            var schedule = seatBooking.Schedule;
            var car = schedule.Car;
            var driver = schedule.Driver;
            var seat = seatBooking.Seat;
            var destinationFrom = schedule.FromDestination;
            var destinationTo = schedule.ToDestination;

            ticketResponses.Add(new TicketResponse(
                ticket.Id,
                ticket.UserId,
                seat.Id,
                schedule.Id,
                (int)schedule.Price,
                destinationFrom?.Province ?? "",
                destinationTo?.Province ?? "",
                seat.Name,
                car.Brand,
                driver.Name,
                schedule.StartTime,
                schedule.ExpectedDuration,
                ticket.CreateAt
            ));
        }

        return ticketResponses;
    }

}