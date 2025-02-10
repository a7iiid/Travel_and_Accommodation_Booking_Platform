using Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Payment.Interfaces;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using System.Net;



public class PayPalService : IPaymentService
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

    public async Task<Order> CreateOrderAsync(decimal amount, string currency)
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
                throw new PaymentException($"PayPal API error: {response.StatusCode}");

            return response.Result<Order>();
        }
        catch (HttpException ex)
        {
            throw new PaymentException($"PayPal API error: {ex.Message}", ex);
        }
    }

    public async Task<Order> CaptureOrderAsync(string orderId)
    {
        try
        {
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());

            var response = await _client.Execute(request);

            if (response.StatusCode != HttpStatusCode.Created)
                throw new PaymentException($"PayPal API error: {response.StatusCode}");

            return response.Result<Order>();
        }
        catch (HttpException ex)
        {
            throw new PaymentException($"PayPal API error: {ex.Message}", ex);
        }
    }
}