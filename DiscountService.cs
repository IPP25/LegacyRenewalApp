using System;

namespace LegacyRenewalApp
{
    public class DiscountService : IDiscountService
    {
        public decimal CalculateDiscount(
            Customer customer, 
            SubscriptionPlan plan, 
            decimal baseAmount, 
            int seatCount, 
            bool useLoyaltyPoints)
        {
            decimal discount = 0m;


            discount += customer.Segment switch
            {
                "Silver" => baseAmount * 0.05m,
                "Gold" => baseAmount * 0.10m,
                "Platinum" => baseAmount * 0.15m,
                "Education" when plan.IsEducationEligible => baseAmount * 0.20m,
                _ => 0m
            };


            if (customer.YearsWithCompany >= 5)
                discount += baseAmount * 0.07m;
            else if (customer.YearsWithCompany >= 2)
                discount += baseAmount * 0.03m;


            discount += seatCount switch
            {
                >= 50 => baseAmount * 0.12m,
                >= 20 => baseAmount * 0.08m,
                >= 10 => baseAmount * 0.04m,
                _ => 0m
            };
            
            if (useLoyaltyPoints && customer.LoyaltyPoints > 0)
            {
                int pointsToUse = Math.Min(customer.LoyaltyPoints, 200);
                discount += pointsToUse;
            }

            return discount;
        }
    }
}