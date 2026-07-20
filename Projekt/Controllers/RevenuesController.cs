using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projekt.Services.Interfaces;

namespace Projekt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RevenuesController : ControllerBase
    {
        private readonly IRevenueService _revenueService;

        public RevenuesController(IRevenueService revenueService)
        {
            _revenueService = revenueService;
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("actual")]
        public async Task<ActionResult<decimal>> GetActualRevenue(CancellationToken cancellationToken)
        {
            var res = await _revenueService.GetActualRevenue(cancellationToken);
            return Ok(res);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("expected")]
        public async Task<ActionResult<decimal>> GetExpectedRevenue(CancellationToken cancellationToken)
        {
            var res = await _revenueService.GetExpectedRevenue(cancellationToken);
            return Ok(res);
        }
    }
}
