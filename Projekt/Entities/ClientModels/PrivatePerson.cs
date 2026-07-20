namespace Projekt.Entities.ClientModels
{
    public class PrivatePerson : Client
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PESEL { get; set; } = string.Empty;
    }
}
