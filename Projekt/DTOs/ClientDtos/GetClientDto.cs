namespace Projekt.DTOs.ClientDtos
{
    public class GetClientDto
    {
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PESEL { get; set; }
        public string? Name { get; set; }
        public string? NIP { get; set; }
        public IEnumerable<GetOrderForClientDto> Orders { get; set; } = new List<GetOrderForClientDto>();

    }
}
