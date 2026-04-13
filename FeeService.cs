
using System;

namespace LegacyRenewalApp
{
    public class FeeService : IFeeService
    {
        public decimal CalculateSupportFee(string planCode, bool includePremiumSupport)
        {
            if (!includePremiumSupport) return 0m;

            return planCode switch
            {
                "START" => 250m,
                "PRO" => 400m,
                "ENTERPRISE" => 700m,
                _ => 0m
            };
        }

        public decimal CalculatePaymentFee(decimal amount, string paymentMethod)
        {
            return paymentMethod switch
            {
                "CARD" => amount * 0.02m,
                "BANK_TRANSFER" => amount * 0.01m,
                "PAYPAL" => amount * 0.035m,
                "INVOICE" => 0m,
                _ => throw new ArgumentException("Unsupported payment method")
            };
        }
    }
}