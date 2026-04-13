namespace LegacyRenewalApp;

public interface ITaxService
{
    decimal CalculateTax(decimal taxableAmount, string country);
}