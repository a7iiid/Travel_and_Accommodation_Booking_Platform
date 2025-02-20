using Application.Services;
using Domain.Enum;
using Domain.Model;
using Infrastructure.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/webhook/paypal")]
    public class PayPalWebhookController : ControllerBase
    {
        
        private readonly PaymentServices _paymentServices;

        public PayPalWebhookController(PaymentServices paymentServices)
        {
            
            _paymentServices = paymentServices;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook([FromBody] JObject payload)
        {
            try
            {
                string eventType = payload["event_type"]?.ToString();
                string orderId = payload["resource"]["id"]?.ToString();
                string paymentStatus = payload["resource"]["status"]?.ToString();
                if (eventType == "PAYMENT.CAPTURE.COMPLETED")
                {
                    

                   await _paymentServices.VerifyAndUpdatePaymentStatusAsync(orderId, PaymentStatus.Completed);
                }
                else if (eventType == "PAYMENT.CAPTURE.DENIED")
                {
                    await _paymentServices.VerifyAndUpdatePaymentStatusAsync(orderId, PaymentStatus.Cancelled);

                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }

}
