using PayPalCheckoutSdk.Core;

namespace PayPal
{
    public class PayPalConfiguration
    {
        public static string ClientId => "YOUR_CLIENT_ID";
        public static string Secret => "YOUR_SECRET";
        public static bool IsSandbox => true;

        public static PayPalEnvironment Environment()
        {
            return IsSandbox
                ? new SandboxEnvironment(ClientId, Secret)
                : new LiveEnvironment(ClientId, Secret);
        }
    }
}
