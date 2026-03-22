public class SeatService : ISeatService
{
    private readonly IUnitOfWork _unitOfWork;

    public SeatService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> IsSeatNameUniqueAsync(string name, int carId)
    {
        var seat = await _unitOfWork.Seats.GetByCarIdAndNameAsync(carId, name);
        return seat == null;
    }
}