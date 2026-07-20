using Projekt.DTOs.AuthDtos;

namespace Projekt.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<TokenDto> Login(LoginDto dto);
        public Task<TokenDto> Refresh(RefreshDto dto);
    }
}
