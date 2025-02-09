

using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

namespace PayPal
{
    public class PaypalServices
    {
        public async Task<string> CreatePayPalOrder(decimal amount, string currency = "USD")
        {
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
        {
            new()
            {
                AmountWithBreakdown = new AmountWithBreakdown
                {
                    CurrencyCode = currency,
                    Value = amount.ToString("F2")
                }
            }
        },
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = "https://your-site.com/success",
                    CancelUrl = "https://your-site.com/cancel"
                }
            });

            var client = new PayPalHttpClient(PayPalConfiguration.Environment());
            var response = await client.Execute(request);
            var statusCode = response.StatusCode;
            var result = response.Result<Order>();

            return result.Links
                .First(x => x.Rel == "approve")
                .Href;
        }
    }
}
