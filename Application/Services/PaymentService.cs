using PayPal.Api;
using Microsoft.Extensions.Configuration;


public class PayPalService
{
    private readonly IConfiguration _config;

    public PayPalService(IConfiguration config)
    {
        _config = config;
    }

    public APIContext GetAPIContext()
    {
        var clientId = _config["PayPal:ClientId"];
        var clientSecret = _config["PayPal:ClientSecret"];
        var config = new Dictionary<string, string> { { "mode", _config["PayPal:Mode"] } };

        var accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
        return new APIContext(accessToken) { Config = config };
    }

    public Payment CreatePayment(decimal amount, string currency, string returnUrl, string cancelUrl)
    {
        var apiContext = GetAPIContext();

        var payment = new Payment
        {
            intent = "sale",
            payer = new Payer { payment_method = "paypal" },
            transactions = new List<Transaction>
            {
                new Transaction
                {
                    amount = new Amount
                    {
                        total = amount.ToString("F2"),
                        currency = currency
                    },
                    description = "Payment for booking"
                }
            },
            redirect_urls = new RedirectUrls
            {
                return_url = returnUrl,
                cancel_url = cancelUrl
            }
        };

        return payment.Create(apiContext);
    }
}
