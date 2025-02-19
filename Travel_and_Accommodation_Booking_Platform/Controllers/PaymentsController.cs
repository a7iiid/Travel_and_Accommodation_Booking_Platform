using Application.DTOs.PaymentDTOs;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Pay.Interfaces;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPayment _paymentService;

        public PaymentsController(IPayment paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDTO request)
        {
            try
            {
                var result = await _paymentService.CreateOrderAsync(
                    request.Amount,
                    request.Currency
                );
                return Ok(new
                {
                    result.OrderId,
                    ApprovalUrl =result.ApprovalUrl
                });
            }
            catch (PaymentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("capture")]
        public async Task<IActionResult> CapturePayment([FromQuery] string orderId)
        {
            try
            {
                var result = await _paymentService.CaptureOrderAsync(orderId);
                return Ok(new { result.Id, result.Status });
            }
            catch (PaymentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
