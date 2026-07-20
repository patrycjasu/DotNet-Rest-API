using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.DTOs.PaymentDtos;
using Projekt.Entities.OrderModels;
using Projekt.Exceptions;
using Projekt.Services.Interfaces;

namespace Projekt.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<PaymentService> _logger;
        public PaymentService(AppDbContext dbContext, ILogger<PaymentService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task AddPayment(int id, AddPaymentDto dto, CancellationToken cancellationToken)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (order == null)
            {
                _logger.LogWarning("Order with id {id} not found", id);
                throw new NotFoundException("Nie znaleziono zamówienia");
            }

            if (order.Date.AddDays(7) < DateTime.UtcNow)
            {
                order.Status = Enums.State.CANCELLED;
            }

            if (order.Status == Enums.State.PAID )
            {
                _logger.LogWarning("Cant pay for already paid order");
                throw new BadRequestException("Nie mozna zaplacic za oplacone zamowienie");

            } else if (order.Status == Enums.State.CANCELLED)
            {
                _logger.LogWarning("Cant pay for cancelledorder");
                throw new BadRequestException("Nie mozna zaplacic za anulowane zamowienie");

            } else if (order.IsDeleted == true)
            {
                _logger.LogWarning("Cant pay for deleted order");
                throw new BadRequestException("Nie mozna zaplacic za usuniete zamowienie");
            }

            if (dto.Amount <= 0)
            {
                _logger.LogWarning("Amount has to be decimal and over 0");
                throw new BadRequestException("Kwota musi byc liczba wieksza od zera");

            }

            var alreadyPaid = await _dbContext.Payments.Where(x => x.OrderId == order.Id).SumAsync(x => x.Amount, cancellationToken);
            var remaining = order.TotalPrice - alreadyPaid;
            
            if (dto.Amount > remaining )
            {
                _logger.LogWarning("Amount cant exceed remaining price");
                throw new BadRequestException("Kwota nie moze przekraczac pozostalej kwoty do zaplaty");
            }

            var payment = new Payment()
            {
                Amount = dto.Amount,
                Date = DateTime.UtcNow,
                Method = dto.Method,
                Order = order
            };

            if (dto.Amount + remaining == order.TotalPrice)
            {
                order.Status = Enums.State.PAID;
            }

            _dbContext.Payments.Add(payment);

            await _dbContext.SaveChangesAsync(cancellationToken);


        }
    }
}
