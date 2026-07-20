namespace Projekt.Services.Interfaces
{
    public interface IRevenueService
    {
        Task<decimal> GetActualRevenue(CancellationToken cancellation);
        Task<decimal> GetExpectedRevenue(CancellationToken cancellation);
    }
}
