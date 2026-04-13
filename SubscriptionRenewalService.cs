using System;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly ISubscriptionRepository _customerRepository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IBillingGateway _billingGateway;
        private readonly IDiscountService _discountService;
        private readonly IFeeService _feeService;
        private readonly ITaxService _taxService;
        private readonly IInvoiceFactory _invoiceFactory;

        
        public SubscriptionRenewalService()
        {
            _customerRepository = new CustomerRepository();
            _planRepository = new SubscriptionPlanRepository();
            _billingGateway = new LegacyBillingGatewayAdapter();
            _discountService = new DiscountService();
            _feeService = new FeeService();
            _taxService = new TaxService();
            _invoiceFactory = new InvoiceFactory();
        }

   
        public SubscriptionRenewalService(
            ISubscriptionRepository customerRepository,
            ISubscriptionPlanRepository planRepository,
            IBillingGateway billingGateway,
            IDiscountService discountService,
            IFeeService feeService,
            ITaxService taxService,
            IInvoiceFactory invoiceFactory)
        {
            _customerRepository = customerRepository;
            _planRepository = planRepository;
            _billingGateway = billingGateway;
            _discountService = discountService;
            _feeService = feeService;
            _taxService = taxService;
            _invoiceFactory = invoiceFactory;
        }

        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            ValidateInput(customerId, planCode, seatCount, paymentMethod);

            var customer = _customerRepository.GetById(customerId);
            var plan = _planRepository.GetByCode(planCode);
            var normalizedPaymentMethod = paymentMethod.Trim().ToUpperInvariant();

            if (!customer.IsActive)
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");

            decimal baseAmount = CalculateBaseAmount(plan, seatCount);
            decimal discountAmount = _discountService.CalculateDiscount(customer, plan, baseAmount, seatCount, useLoyaltyPoints);

            decimal subtotalAfterDiscount = Math.Max(baseAmount - discountAmount, 300m);

            decimal supportFee = _feeService.CalculateSupportFee(plan.Code, includePremiumSupport);
            decimal paymentFee = _feeService.CalculatePaymentFee(subtotalAfterDiscount + supportFee, normalizedPaymentMethod);

            decimal taxBase = subtotalAfterDiscount + supportFee + paymentFee;
            decimal taxAmount = _taxService.CalculateTax(taxBase, customer.Country);

            decimal finalAmount = taxBase + taxAmount;
            if (finalAmount < 500m)
                finalAmount = 500m;

            var invoice = _invoiceFactory.CreateInvoice(
                customer, plan, seatCount, normalizedPaymentMethod,
                baseAmount, discountAmount, supportFee, paymentFee,
                taxAmount, finalAmount, includePremiumSupport, useLoyaltyPoints);

            _billingGateway.SaveInvoice(invoice);

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                _billingGateway.SendEmail(
                    customer.Email,
                    "Subscription renewal invoice",
                    $"Hello {customer.FullName}, your renewal for plan {plan.Code} " +
                    $"has been prepared. Final amount: {invoice.FinalAmount:F2}.");
            }

            return invoice;
        }

        private static void ValidateInput(int customerId, string planCode, int seatCount, string paymentMethod)
        {
            if (customerId <= 0) throw new ArgumentException("Customer id must be positive");
            if (string.IsNullOrWhiteSpace(planCode)) throw new ArgumentException("Plan code is required");
            if (seatCount <= 0) throw new ArgumentException("Seat count must be positive");
            if (string.IsNullOrWhiteSpace(paymentMethod)) throw new ArgumentException("Payment method is required");
        }

        private static decimal CalculateBaseAmount(SubscriptionPlan plan, int seatCount)
        {
            return (plan.MonthlyPricePerSeat * seatCount * 12m) + plan.SetupFee;
        }
    }
}