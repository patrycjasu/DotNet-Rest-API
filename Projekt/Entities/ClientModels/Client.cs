using Projekt.Entities.OrderModels;

namespace Projekt.Entities.ClientModels
{
    public class Client
    {
        public int Id { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
    }
}
