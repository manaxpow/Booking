public record UpdateCarRequest(int Id, string LicensePlate, int Capacity, string Brand, List<UpdateSeatRequest> Seats);

