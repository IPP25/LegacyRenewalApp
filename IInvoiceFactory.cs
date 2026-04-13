namespace LegacyRenewalApp;

public interface IInvoiceFactory
{
    RenewalInvoice CreateInvoice(
        Customer customer,
        SubscriptionPlan plan,
        int seatCount,
        string paymentMethod,
        decimal baseAmount,
        decimal discountAmount,
        decimal supportFee,
        decimal paymentFee,
        decimal taxAmount,
        decimal finalAmount,
        bool includePremiumSupport,
        bool useLoyaltyPoints);
}