using Projekt.Entities.ClientModels;

namespace Projekt.Entities.OrderModels
{
    public class Order
    {
        public int Id { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal TotalPrice { get; set; }
        public int ShipmentId { get; set; }
        public int ClientId { get; set; }
        public DateTime Date {  get; set; }
        public Enums.State Status { get; set; }
        public bool IsDeleted { get; set; }

        public Shipment Shipment { get; set; }
        public Client Client { get; set; }
        public List<Payment> Payments { get; set; } = new List<Payment>();
        public List<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();


    }
}
