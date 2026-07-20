namespace Projekt.DTOs.PaymentDtos
{
    public class GetPaymentDto
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Enums.Method Method { get; set; }
    }
}
