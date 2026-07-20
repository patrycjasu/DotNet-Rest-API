namespace Projekt.DTOs.ClientDtos
{
    public class GetClientCompanyDto
    {
        public int? Id { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Name { get; set; }
        public string? NIP { get; set; }
        public IEnumerable<GetOrderForClientDto>? Orders { get; set; } = new List<GetOrderForClientDto>();
    }
}
