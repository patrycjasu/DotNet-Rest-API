using Projekt.Entities.ClientModels;
using Projekt.Entities.OrderModels;

namespace Projekt.DTOs.OrderDtos
{
    public class AddOrderDto
    {
        public int ClientId { get; set; }
        public IEnumerable<AddUpdateProductInOrderDto> ProductsOrders { get; set; } = new List<AddUpdateProductInOrderDto>();
    }
}
