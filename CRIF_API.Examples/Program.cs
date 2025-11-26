using CRIF_API.Client.Configuration;
using CRIF_API.Client.Enums;
using CRIF_API.Client.Models.Common;
using CRIF_API.Client.Models.Requests;
using CRIF_API.Client.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CRIF_API.Examples;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== CRIF Credit Bureau API Examples ===\n");

        // Load configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.local.json", optional: true) // For local credentials
            .Build();

        // Setup dependency injection
        var services = new ServiceCollection();
        services.Configure<CrifSettings>(configuration.GetSection("CrifSettings"));
        services.AddSingleton<ISoapClient, SoapClient>();
        services.AddScoped<ICrifCreditBureauService, CrifCreditBureauService>();

        var serviceProvider = services.BuildServiceProvider();

        // Get service
        var crifService = serviceProvider.GetRequiredService<ICrifCreditBureauService>();

        // Display menu
        while (true)
        {
            Console.WriteLine("\nChoose an example:");
            Console.WriteLine("1. NAE - New Application Enquiry (Individual)");
            Console.WriteLine("2. NAE - New Application Enquiry (Company)");
            Console.WriteLine("3. ME - Monitoring Enquiry");
            Console.WriteLine("4. AUE - Application Update Enquiry");
            Console.WriteLine("5. PAE - Portfolio Alerts Enquiry");
            Console.WriteLine("6. NAEP - New Application Enquiry with PDF");
            Console.WriteLine("0. Exit");
            Console.Write("\nEnter choice: ");

            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        await Example_NAE_Individual(crifService);
                        break;
                    case "2":
                        await Example_NAE_Company(crifService);
                        break;
                    case "3":
                        await Example_ME(crifService);
                        break;
                    case "4":
                        await Example_AUE(crifService);
                        break;
                    case "5":
                        await Example_PAE(crifService);
                        break;
                    case "6":
                        await Example_NAEP(crifService);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner: {ex.InnerException.Message}");
            }
        }
    }

    static async Task Example_NAE_Individual(ICrifCreditBureauService service)
    {
        Console.WriteLine("\n=== NAE - Individual Example ===");

        var request = new NAERequest
        {
            ProviderSubjectNo = "IND12345",
            Individual = new Individual
            {
                FirstName = "JOHN",
                LastName = "DOE",
                Patronymic = "SMITH",
                DateOfBirth = new DateTime(1990, 5, 15),
                Gender = Gender.Male,
                Addresses = new List<Address>
                {
                    new Address
                    {
                        AddressType = AddressType.Residential,
                        StreetNameAndNumber = "123 Main Street",
                        City = "TASHKENT",
                        District = "UCHTEPA",
                        Region = "TASHKENT_CITY",
                        Country = "UZ"
                    }
                },
                IdentificationCodes = new List<IdentificationCode>
                {
                    new IdentificationCode
                    {
                        IdentificationType = "PINFL",
                        IdentificationNumber = "12345678901234"
                    }
                },
                IdDocuments = new List<IdDocument>
                {
                    new IdDocument
                    {
                        IdType = "PASSPORT_UZ",
                        IdNumber = "AA1234567",
                        IssueDate = new DateTime(2020, 1, 1)
                    }
                },
                Contacts = new List<Contact>
                {
                    new Contact
                    {
                        ContactType = "MOBILE",
                        ContactValue = "+998901234567"
                    }
                }
            },
            Application = new ApplicationData
            {
                ContractType = "01", // See domain tables
                ContractPhase = ContractPhase.Requested,
                ContractRequestDate = DateTime.Now.Date,
                Currency = "UZS",
                FinancedAmount = 50000000, // 50M UZS
                MonthlyPaymentAmount = 5000000, // 5M UZS
                DueDate = DateTime.Now.AddYears(1).Date
            },
            Link = new LinkData
            {
                RoleOfSubject = SubjectRole.Borrower
            },
            ApplicationCodes = new ApplicationCodes
            {
                ProviderContractNo = "LOAN-2024-001"
            }
        };

        Console.WriteLine("Sending NAE request...");
        var response = await service.NewApplicationEnquiryAsync(request);

        Console.WriteLine($"\n✅ Success: {response.Success}");
        if (response.Success)
        {
            Console.WriteLine($"CB Contract Code: {response.ApplicationCodes?.CBContractCode}");
            Console.WriteLine($"CB Subject Code: {response.ApplicationCodes?.CBSubjectCode}");
            Console.WriteLine($"Match Found: {response.CreditReport?.MatchedSubject?.FlagMatched}");

            if (response.CreditReport?.ContractHistory != null)
            {
                var history = response.CreditReport.ContractHistory;
                Console.WriteLine($"\nContract History:");
                Console.WriteLine($"  Total Contracts: {history.AggregatedData?.TotalContracts ?? 0}");
                Console.WriteLine($"  Total Providers: {history.AggregatedData?.TotalProviders ?? 0}");
                Console.WriteLine($"  Active Contracts: {history.GrantedContracts?.Count ?? 0}");
            }
        }
        else
        {
            Console.WriteLine($"Error: {response.Error?.ErrorCode} - {response.Error?.ErrorDescription}");
        }
    }

    static async Task Example_NAE_Company(ICrifCreditBureauService service)
    {
        Console.WriteLine("\n=== NAE - Company Example ===");

        var request = new NAERequest
        {
            ProviderSubjectNo = "COM67890",
            Company = new Company
            {
                TradeName = "ACME CORPORATION LLC",
                ShortCompanyName = "ACME LLC",
                LegalForm = "LLC", // See domain tables
                RegistrationPlace = "TASHKENT",
                EconomicActivity = "62.01", // See domain tables
                EstablishmentDate = new DateTime(2010, 3, 15),
                GrossIncome = 500000000,
                Addresses = new List<Address>
                {
                    new Address
                    {
                        AddressType = AddressType.Work,
                        StreetNameAndNumber = "456 Business Avenue",
                        City = "TASHKENT",
                        District = "YUNUSABAD",
                        Region = "TASHKENT_CITY",
                        Country = "UZ"
                    }
                },
                IdentificationCodes = new List<IdentificationCode>
                {
                    new IdentificationCode
                    {
                        IdentificationType = "TIN",
                        IdentificationNumber = "123456789"
                    }
                }
            },
            Application = new ApplicationData
            {
                ContractType = "05", // Business loan
                ContractPhase = ContractPhase.Requested,
                ContractRequestDate = DateTime.Now.Date,
                Currency = "UZS",
                FinancedAmount = 500000000, // 500M UZS
                DueDate = DateTime.Now.AddYears(3).Date
            },
            Link = new LinkData
            {
                RoleOfSubject = SubjectRole.Borrower
            },
            ApplicationCodes = new ApplicationCodes
            {
                ProviderContractNo = "BIZ-LOAN-2024-001"
            }
        };

        Console.WriteLine("Sending NAE request for company...");
        var response = await service.NewApplicationEnquiryAsync(request);

        Console.WriteLine($"\n✅ Success: {response.Success}");
        Console.WriteLine($"CB Contract Code: {response.ApplicationCodes?.CBContractCode}");
    }

    static async Task Example_ME(ICrifCreditBureauService service)
    {
        Console.WriteLine("\n=== ME - Monitoring Enquiry Example ===");
        Console.Write("Enter CB Subject Code or Provider Subject No: ");
        var subjectCode = Console.ReadLine();

        var request = new MERequest
        {
            CBSubjectCode = subjectCode,
            ProductRole = ProductRole.Full,
            RequestScore = true
        };

        Console.WriteLine("Sending ME request...");
        var response = await service.MonitoringEnquiryAsync(request);

        Console.WriteLine($"\n✅ Success: {response.Success}");
        if (response.Success && response.CreditReport != null)
        {
            Console.WriteLine($"Match Found: {response.CreditReport.MatchedSubject?.FlagMatched}");

            if (response.Score != null)
            {
                Console.WriteLine($"Credit Score: {response.Score.ScoreValue}");
            }
        }
    }

    static async Task Example_AUE(ICrifCreditBureauService service)
    {
        Console.WriteLine("\n=== AUE - Application Update Example ===");
        Console.Write("Enter CB Contract Code: ");
        var contractCode = Console.ReadLine();

        var request = new AUERequest
        {
            ApplicationCodes = new ApplicationCodes
            {
                CBContractCode = contractCode
            },
            ApplicationToUpdate = new ApplicationToUpdate
            {
                FinancedAmount = 60000000, // Update to 60M UZS
                ContractPhase = ContractPhase.Active
            }
        };

        Console.WriteLine("Sending AUE request...");
        var response = await service.ApplicationUpdateEnquiryAsync(request);

        Console.WriteLine($"\n✅ Success: {response.Success}");
        if (response.Success)
        {
            Console.WriteLine("Application updated successfully!");
            Console.WriteLine($"Previous amount: {response.ApplicationDB?.FinancedAmount}");
            Console.WriteLine($"Updated amount: {response.ApplicationUpdated?.FinancedAmount}");
        }
    }

    static async Task Example_PAE(ICrifCreditBureauService service)
    {
        Console.WriteLine("\n=== PAE - Portfolio Alerts Example ===");

        var request = new PAERequest
        {
            DateFrom = DateTime.Now.AddDays(-30),
            DateTo = DateTime.Now,
            MaxResults = 100
        };

        Console.WriteLine("Sending PAE request...");
        var response = await service.PortfolioAlertsEnquiryAsync(request);

        Console.WriteLine($"\n✅ Success: {response.Success}");
        if (response.Success)
        {
            Console.WriteLine($"Total Alerts: {response.TotalCount}");

            foreach (var alert in response.Alerts.Take(5))
            {
                Console.WriteLine($"\n  Alert: {alert.AlertCode}");
                Console.WriteLine($"  Date: {alert.EventDateTime}");
                Console.WriteLine($"  Subject: {alert.SubjectName}");
                Console.WriteLine($"  Details: {alert.Details}");
            }
        }
    }

    static async Task Example_NAEP(ICrifCreditBureauService service)
    {
        Console.WriteLine("\n=== NAEP - NAE with PDF Example ===");
        Console.WriteLine("This returns the same data as NAE plus a PDF document.");

        var request = new NAEPRequest
        {
            ProviderSubjectNo = "TEST123",
            PresentationCulture = PresentationCulture.Russian,
            // ... (same as NAE request)
        };

        Console.WriteLine("Sending NAEP request...");
        var response = await service.NewApplicationEnquiryPresentationAsync(request);

        Console.WriteLine($"\n✅ Success: {response.Success}");
        if (response.Success && response.PresentationDocument != null)
        {
            // Save PDF
            var pdfBytes = Convert.FromBase64String(response.PresentationDocument);
            var fileName = $"credit_report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            await File.WriteAllBytesAsync(fileName, pdfBytes);
            Console.WriteLine($"PDF saved to: {fileName}");
        }
    }
}
