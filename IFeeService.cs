namespace LegacyRenewalApp;

public interface IFeeService
{
    decimal CalculateSupportFee(string planCode, bool includePremiumSupport);
    decimal CalculatePaymentFee(decimal amount, string paymentMethod);
}