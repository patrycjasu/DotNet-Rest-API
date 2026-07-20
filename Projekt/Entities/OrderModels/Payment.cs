namespace Projekt.Entities.OrderModels
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Enums.Method Method { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
