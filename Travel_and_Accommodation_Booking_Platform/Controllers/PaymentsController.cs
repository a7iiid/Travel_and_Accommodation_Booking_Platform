using Application.DTOs.PaymentDTOs;
using Application.Services;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentServices _paymentService;

        public PaymentsController(PaymentServices paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentDTO paymentDTO)
        {
            try
            {
                var result = await _paymentService.AddPaymentAsync(
                    paymentDTO
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

        

    }
}
