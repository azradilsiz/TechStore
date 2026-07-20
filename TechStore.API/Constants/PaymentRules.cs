namespace TechStore.API.Constants
{
    public static class PaymentRules
    {
        public const string CreditCard = "Credit Card";
        public const string BankTransfer = "Bank Transfer";
        public const string Cash = "Cash";

        public const string Pending = "Pending";
        public const string Paid = "Paid";
        public const string PayOnDelivery = "PayOnDelivery";
        public const string Failed = "Failed";
        public const string Refunded = "Refunded";
        public const string Cancelled = "Cancelled";

        public static readonly string[] ValidMethods =
            [CreditCard, BankTransfer, Cash];

        public static readonly string[] ValidStatuses =
            [Pending, Paid, PayOnDelivery, Failed, Refunded, Cancelled];

        public static string GetInitialStatus(string paymentMethod)
        {
            return paymentMethod switch
            {
                CreditCard => Paid,
                Cash => PayOnDelivery,
                _ => Pending
            };
        }
    }
}
