using Chapter6.DataAccess.Context;
using Chapter6.Models;
using Microsoft.EntityFrameworkCore;

namespace Chapter6.DataAccess.Repositories;

public class DriverRepository : GenericRepository<Driver>, IDriverRepository
{
    public DriverRepository(AppDbContext context)
        : base(context) { }
}
