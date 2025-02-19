using Domain.Exceptions;
using Domain.Model;
using Microsoft.Extensions.Configuration;
using Pay.Interfaces;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using System.Net;



public class PayPalService : IPaymentGateway
{
    private readonly PayPalHttpClient _client;
    private readonly string _returnUrl;
    private readonly string _cancelUrl;

    public PayPalService(IConfiguration config)
    {
        var clientId = config["PayPal:ClientId"];
        var secret = config["PayPal:Secret"];

        PayPalEnvironment environment = config["PayPal:Environment"]?.ToLower() == "live"
            ? new LiveEnvironment(clientId, secret)
            : new SandboxEnvironment(clientId, secret);

        _client = new PayPalHttpClient(environment);
        _returnUrl = config["PayPal:ReturnUrl"];
        _cancelUrl = config["PayPal:CancelUrl"];
    }

    public async Task<CreateOrderResult> CreateOrderAsync(decimal amount, string currency)
    {
        try
        {
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");

            var orderRequest = new OrderRequest
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
                    ReturnUrl = _returnUrl,
                    CancelUrl = _cancelUrl
                }
            };

            request.RequestBody(orderRequest);
            var response = await _client.Execute(request);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new PaymentException($"PayPal API error: {response.StatusCode}");
            }
            var order= response.Result<Order>();
            return new CreateOrderResult {OrderId= order.Id,
                ApprovalUrl= order.Links.First(x => x.Rel == "approve").Href,
                PaymentMethod=Domain.Enum.PaymentMethod.PayPal
            };
        }
        catch (HttpException ex)
        {
            throw new PaymentException($"PayPal API error: {ex.Message}", ex);
        }
    }

    public async Task<Order?> GetOrderStatusAsync(string orderId)
    {
        try
        {
            var request = new OrdersGetRequest(orderId);
            var response = await _client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new PaymentException($"PayPal API error: {response.StatusCode}");
            }

            return response.Result<Order>();
        }
        catch (HttpException ex)
        {
            throw new PaymentException($"PayPal API error: {ex.Message}", ex);
        }
    }

}
