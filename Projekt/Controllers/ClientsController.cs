using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projekt.DTOs.ClientDtos;
using Projekt.Entities.ClientModels;
using Projekt.Services.Interfaces;

namespace Projekt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientsController : ControllerBase
    {

        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }
        [Authorize]
        [HttpPost("company")]
        public async Task<IActionResult> AddCompany([FromBody] AddClientCompanyDto dto, CancellationToken cancellationToken)
        {
            await _clientService.AddClientCompany(dto, cancellationToken);
            return Created();
        }
        [Authorize]
        [HttpPost("person")]
        public async Task<IActionResult> AddPerson([FromBody] AddClientPersonDto dto, CancellationToken cancellationToken)
        {
            await _clientService.AddClientPerson(dto, cancellationToken);
            return Created();
        }
        [Authorize]
        [HttpPut("{id}/company")]
        public async Task<IActionResult> UpdateCompany( int id, [FromBody] UpdateClientCompanyDto dto,  CancellationToken cancellationToken)
        {
            await _clientService.UpdateClientCompany(id, dto, cancellationToken);
            return NoContent();
        }
        [Authorize]
        [HttpPut("{id}/person")]
        public async Task<IActionResult> UpdatePerson( int id, [FromBody] UpdateClientPersonDto dto, CancellationToken cancellationToken)
        {
            await _clientService.UpdateClientPerson(id, dto, cancellationToken);
            return NoContent();
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id, CancellationToken cancellationToken)
        {

            var res = await _clientService.GetClientById(id, cancellationToken);
            return Ok(res);
        }
        [Authorize]
        [HttpGet("company")]
        public async Task<ActionResult> GetCompanies([FromQuery]string? name, CancellationToken cancellationToken)
        {
            var res = await _clientService.GetClientsCompanies(name, cancellationToken);
            return Ok(res);
        }
        [Authorize]
        [HttpGet("person")]
        public async Task<ActionResult> GetPeople([FromQuery] string? firstName, [FromQuery] string? lastName, CancellationToken cancellationToken)
        {
            var res = await _clientService.GetClientsPeople(firstName, lastName, cancellationToken);
            return Ok(res);
        }
    }
}
