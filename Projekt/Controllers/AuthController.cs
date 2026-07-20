using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projekt.DTOs.AuthDtos;
using Projekt.Services.Interfaces;

namespace Projekt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private IAuthService _service;
        public AuthController(IAuthService service)
        {
            _service = service;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto dto)
        {
            var tokens = await _service.Login(dto);
            return Ok(tokens);
        }
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task <ActionResult> Refresh(RefreshDto dto)
        {
            var tokens = await _service.Refresh(dto);
            return Ok(tokens);
        }

    }
}
