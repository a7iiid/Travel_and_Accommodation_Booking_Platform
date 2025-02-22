using Domain.Exceptions;
using Domain.Model;
using Microsoft.Extensions.Configuration;
using Pay.Interfaces;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using System.Net;
using System;



public class PayPalGateWay : IPaymentGateway
{
    private readonly PayPalHttpClient _client;
    private readonly string _returnUrl;
    private readonly string _cancelUrl;

    public PayPalGateWay()
    {
        var clientId = System.Environment.GetEnvironmentVariable("ClientId");
        var secret = System.Environment.GetEnvironmentVariable("Secret");

        PayPalEnvironment environment = System.Environment.GetEnvironmentVariable("Environment")?.ToLower() == "live"
            ? new LiveEnvironment(clientId, secret)
            : new SandboxEnvironment(clientId, secret);

        _client = new PayPalHttpClient(environment);
        _returnUrl = System.Environment.GetEnvironmentVariable("ReturnUrl");
        _cancelUrl = System.Environment.GetEnvironmentVariable("CancelUrl");
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

    

}
