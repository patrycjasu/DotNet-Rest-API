using Projekt.DTOs.OrderDtos;

namespace Projekt.Services.Interfaces
{
    public interface IOrderService
    {
        Task<GetOrderDto> GetOrderById(int id, CancellationToken cancellationToken);
        Task<IEnumerable<GetOrderDto>> GetAllOrders(bool? active, CancellationToken cancellationToken);
        Task AddOrder(AddOrderDto dto, CancellationToken cancellationToken);
        Task UpdateOrder(UpdateOrderDto dto, int id, CancellationToken cancellationToken);
        Task DeleteOrder (int id, CancellationToken cancellationToken);
    }
}
