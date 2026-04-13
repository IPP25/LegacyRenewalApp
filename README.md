could not launch the app on McBook becuase of the Net version 0.8 
the launching od the LegacyRenewalAppConsumer definitely works trough console

step by step:
cd (name of the directory)
dotnet clean
dotnet restore
dotnet build

then dotnet run --project LegacyRenewalAppConsumer/LegacyRenewalAppConsumer.csproj 

OUTPUT:

Invoice INV-20260413-3-PRO saved for John Smith
Email sent to john.smith@example.com: Subscription renewal invoice
Invoice created successfully
InvoiceNumber=INV-20260413-3-PRO, Customer=John Smith, Plan=PRO, Seats=18, FinalAmount=17671.67, Notes=platinum discount; long-term loyalty discount; small team discount; loyalty points used: 200; premium support included; card payment fee;
Final amount: 17671.67
igorpytlinski@Igors-MacBook-Air zadanie_refactoring_renewal-2 % 
