using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projekt.DTOs.OrderDtos;
using Projekt.DTOs.PaymentDtos;
using Projekt.Services.Interfaces;

namespace Projekt.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;

        public OrdersController(IOrderService orderService, IPaymentService paymentService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] AddOrderDto dto, CancellationToken cancellationToken)
        {
            await _orderService.AddOrder(dto, cancellationToken);
            return Created();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _orderService.DeleteOrder(id, cancellationToken);
            return NoContent();
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetAllOrders([FromQuery] bool? active, CancellationToken cancellationToken)
        {
            var res = await _orderService.GetAllOrders(active, cancellationToken);
            return Ok(res);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetOrderById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var res = await _orderService.GetOrderById(id, cancellationToken);
            return Ok(res);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder([FromRoute] int id, [FromBody] UpdateOrderDto dto, CancellationToken cancellationToken)
        {
            await _orderService.UpdateOrder(dto, id, cancellationToken);
            return NoContent() ;
        }
        [Authorize]
        [HttpPost("{id}/payment")]
        public async Task<IActionResult> AddPayment([FromRoute] int id, [FromBody] AddPaymentDto dto, CancellationToken cancellation)
        {
            await _paymentService.AddPayment(id, dto, cancellation);
            return NoContent();
        }


    }
}
