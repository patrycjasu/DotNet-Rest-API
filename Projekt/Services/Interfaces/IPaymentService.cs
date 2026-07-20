using Projekt.DTOs.PaymentDtos;

namespace Projekt.Services.Interfaces
{
    public interface IPaymentService
    {
        Task AddPayment(int id, AddPaymentDto dto, CancellationToken cancellationToken);
    }
}
