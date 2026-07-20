using Microsoft.EntityFrameworkCore;
using Projekt.Data;

namespace Projekt.Services
{
    public class OrderExpirationService : BackgroundService
    {
        private readonly ILogger<OrderExpirationService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public OrderExpirationService(ILogger<OrderExpirationService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            while (!cancellationToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await context.Database.ExecuteSqlRawAsync("exec CancelExpiredOrders", cancellationToken);
                _logger.LogInformation("expired orders checked");
                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
            }
        }
    }
}
