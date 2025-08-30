namespace DesignPatterns.Structural.Adapter
{
    // Target interface (what client expects)
    public interface ITarget
    {
        string GetRequest();
    }

    // Adaptee (existing class with incompatible interface)
    public class Adaptee
    {
        public string GetSpecificRequest()
        {
            return "Special request from Adaptee";
        }
    }

    // Adapter (makes Adaptee compatible with Target interface)
    public class Adapter : ITarget
    {
        private readonly Adaptee _adaptee;

        public Adapter(Adaptee adaptee)
        {
            _adaptee = adaptee;
        }

        public string GetRequest()
        {
            return $"Adapter: {_adaptee.GetSpecificRequest()}";
        }
    }

    // Real-world example: Payment systems
    public interface IPaymentProcessor
    {
        string ProcessPayment(decimal amount);
    }

    // Third-party payment system (Adaptee)
    public class PayPalApi
    {
        public string MakePayment(decimal amount)
        {
            return $"PayPal payment of ${amount} processed";
        }
    }

    // Adapter for PayPal
    public class PayPalAdapter : IPaymentProcessor
    {
        private readonly PayPalApi _payPalApi;

        public PayPalAdapter(PayPalApi payPalApi)
        {
            _payPalApi = payPalApi;
        }

        public string ProcessPayment(decimal amount)
        {
            return _payPalApi.MakePayment(amount);
        }
    }
}