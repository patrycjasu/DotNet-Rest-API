namespace Projekt.DTOs.PaymentDtos
{
    public class AddPaymentDto
    {
        public decimal Amount { get; set; }
        public Enums.Method Method { get; set; }
    }
}
