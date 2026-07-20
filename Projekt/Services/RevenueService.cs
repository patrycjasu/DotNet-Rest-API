using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.Services.Interfaces;

namespace Projekt.Services
{
    public class RevenueService : IRevenueService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<RevenueService> _logger;
        public RevenueService(AppDbContext dbContext, ILogger<RevenueService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task<decimal> GetActualRevenue(CancellationToken cancellation)
        {
            var total = await _dbContext.Orders.Where(x => x.Status == Enums.State.PAID).SumAsync(x => x.TotalPrice, cancellation);
            return total;
        }

        public async Task<decimal> GetExpectedRevenue(CancellationToken cancellation)
        {
            var total = await _dbContext.Orders.Where(x => x.Status == Enums.State.PAID || (x.Status == Enums.State.ACTIVE & x.IsDeleted == false)).SumAsync(x => x.TotalPrice, cancellation);
            return total;
        }
    }
}
