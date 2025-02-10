using Application.DTOs.PaymentDTOs;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Payment.Interfaces;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
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
                    result.Id,
                    ApprovalUrl = result.Links.First(l => l.Rel == "approve").Href
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
