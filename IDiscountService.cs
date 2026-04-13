namespace LegacyRenewalApp
{
    public interface IDiscountService
    {
        decimal CalculateDiscount(
            Customer customer, 
            SubscriptionPlan plan, 
            decimal baseAmount, 
            int seatCount, 
            bool useLoyaltyPoints);
    }
}