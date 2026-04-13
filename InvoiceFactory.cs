using System;
using System.Collections.Generic;

namespace LegacyRenewalApp
{
    public class InvoiceFactory : IInvoiceFactory
    {
        public RenewalInvoice CreateInvoice(
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
            bool useLoyaltyPoints)
        {
            string notes = BuildNotes(customer, plan, seatCount, includePremiumSupport, useLoyaltyPoints, paymentMethod);

            return new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customer.Id}-{plan.Code}",
                CustomerName = customer.FullName,
                PlanCode = plan.Code,
                PaymentMethod = paymentMethod,
                SeatCount = seatCount,
                BaseAmount = Math.Round(baseAmount, 2, MidpointRounding.AwayFromZero),
                DiscountAmount = Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero),
                SupportFee = Math.Round(supportFee, 2, MidpointRounding.AwayFromZero),
                PaymentFee = Math.Round(paymentFee, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero),
                FinalAmount = Math.Round(finalAmount, 2, MidpointRounding.AwayFromZero),
                Notes = notes.Trim(),
                GeneratedAt = DateTime.UtcNow
            };
        }

        private static string BuildNotes(Customer customer, SubscriptionPlan plan, int seatCount, 
            bool includePremiumSupport, bool useLoyaltyPoints, string paymentMethod)
        {
            var notes = new List<string>();

            if (customer.Segment == "Silver") notes.Add("silver discount");
            else if (customer.Segment == "Gold") notes.Add("gold discount");
            else if (customer.Segment == "Platinum") notes.Add("platinum discount");
            else if (customer.Segment == "Education" && plan.IsEducationEligible) notes.Add("education discount");

            if (customer.YearsWithCompany >= 5) notes.Add("long-term loyalty discount");
            else if (customer.YearsWithCompany >= 2) notes.Add("basic loyalty discount");

            if (seatCount >= 50) notes.Add("large team discount");
            else if (seatCount >= 20) notes.Add("medium team discount");
            else if (seatCount >= 10) notes.Add("small team discount");

            if (useLoyaltyPoints && customer.LoyaltyPoints > 0)
                notes.Add($"loyalty points used: {Math.Min(customer.LoyaltyPoints, 200)}");

            if (includePremiumSupport) notes.Add("premium support included");

            notes.Add(paymentMethod.ToLower() switch
            {
                "card" => "card payment fee",
                "bank_transfer" => "bank transfer fee",
                "paypal" => "paypal fee",
                "invoice" => "invoice payment",
                _ => "payment fee"
            });

            return string.Join("; ", notes) + "; ";
        }
    }
}