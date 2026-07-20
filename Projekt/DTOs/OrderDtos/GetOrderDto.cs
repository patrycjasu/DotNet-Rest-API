using Projekt.DTOs.PaymentDtos;

namespace Projekt.DTOs.OrderDtos
{
    public class GetOrderDto
    {
        public int Id { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal TotalPrice { get; set; }
        public string? ShipmentName { get; set; }
        public int ClientId { get; set;  }
        public DateTime Date { get; set; }
        public Enums.State Status { get; set; }
        public bool IsDeleted { get; set; }
        public List<GetProductInOrderDto> ProductsOrders { get; set; } = new List<GetProductInOrderDto>();
        public List<GetPaymentDto> Payments { get; set; } = new List<GetPaymentDto>();

    }
}
