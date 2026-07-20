namespace Projekt.Entities.AuthModels
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string HashPassword { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? RefreshToken {  get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
    }
}
