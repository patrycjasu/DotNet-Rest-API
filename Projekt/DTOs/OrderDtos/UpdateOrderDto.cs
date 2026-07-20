namespace Projekt.DTOs.OrderDtos
{
    public class UpdateOrderDto
    {
        public IEnumerable<AddUpdateProductInOrderDto> ProductsOrders { get; set; } = new List<AddUpdateProductInOrderDto>();
    }
}
