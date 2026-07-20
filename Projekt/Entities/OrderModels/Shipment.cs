namespace Projekt.Entities.OrderModels
{
    public class Shipment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal MaxWeight { get; set; }
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();

    }
}
