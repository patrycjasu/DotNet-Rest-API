namespace Projekt.DTOs.AuthDtos
{
    public class TokenDto
    {
        public string AccessToken { get; set;} = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;

        public TokenDto(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
