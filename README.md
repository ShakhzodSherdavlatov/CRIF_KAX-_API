# CRIF Credit Bureau API Client (C#)

A C# client library for integrating with the CRIF KAX Credit Bureau (Uzbekistan) SOAP API.

## Overview

This library provides a type-safe, easy-to-use interface for querying the CRIF Credit Bureau system. It supports all major enquiry types:

- **NAE** - New Application Enquiry
- **ME** - Monitoring Enquiry
- **AUE** - Application Update Enquiry
- **PAE** - Portfolio Alerts Enquiry
- **NAEP/MEP** - Presentation queries (with PDF output)
- **RCU** - Realtime Contract Update (TODO)

## Project Structure

```
CRIF_API/
├── CRIF_API.Client/          # Main client library
│   ├── Configuration/         # Settings and configuration
│   ├── Enums/                # Enumerations (ContractPhase, SubjectRole, etc.)
│   ├── Exceptions/           # Custom exceptions
│   ├── Models/               # DTOs
│   │   ├── Common/          # Shared models (Individual, Company, Address, etc.)
│   │   ├── Requests/        # Request models
│   │   └── Responses/       # Response models
│   └── Services/            # Service interfaces and implementations
└── CRIF_API.Examples/       # Example console application
```

## Prerequisites

- .NET 8.0 or higher
- CRIF Credit Bureau credentials (User ID & Password)
- Access to CRIF UAT or Production endpoint

## Installation

### Option 1: Add as Project Reference

```bash
dotnet add reference path/to/CRIF_API.Client/CRIF_API.Client.csproj
```

### Option 2: Build and Use DLL

```bash
cd CRIF_API.Client
dotnet build -c Release
# Reference the DLL from bin/Release/net8.0/
```

## Configuration

### appsettings.json

```json
{
  "CrifSettings": {
    "EndpointUrl": "https://uat.crifkax.uz/soap/endpoint",
    "UserId": "YOUR_USER_ID",
    "Password": "YOUR_PASSWORD",
    "TimeoutSeconds": 120,
    "Environment": "UAT",
    "EnableLogging": true
  }
}
```

### Dependency Injection Setup

```csharp
using CRIF_API.Client.Configuration;
using CRIF_API.Client.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Configure settings
services.Configure<CrifSettings>(configuration.GetSection("CrifSettings"));

// Register services
services.AddSingleton<ISoapClient, SoapClient>();
services.AddScoped<ICrifCreditBureauService, CrifCreditBureauService>();

var serviceProvider = services.BuildServiceProvider();
var crifService = serviceProvider.GetRequiredService<ICrifCreditBureauService>();
```

## Quick Start Examples

### 1. New Application Enquiry (Individual)

```csharp
var request = new NAERequest
{
    ProviderSubjectNo = "IND12345",
    Individual = new Individual
    {
        FirstName = "JOHN",
        LastName = "DOE",
        DateOfBirth = new DateTime(1990, 5, 15),
        Addresses = new List<Address>
        {
            new Address
            {
                AddressType = AddressType.Residential,
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
        }
    },
    Application = new ApplicationData
    {
        ContractType = "01",
        ContractPhase = ContractPhase.Requested,
        Currency = "UZS",
        FinancedAmount = 50000000,
        MonthlyPaymentAmount = 5000000
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

var response = await crifService.NewApplicationEnquiryAsync(request);

if (response.Success)
{
    Console.WriteLine($"CB Contract Code: {response.ApplicationCodes.CBContractCode}");
    Console.WriteLine($"CB Subject Code: {response.ApplicationCodes.CBSubjectCode}");
    Console.WriteLine($"Match Found: {response.CreditReport.MatchedSubject.FlagMatched}");
}
```

### 2. New Application Enquiry (Company)

```csharp
var request = new NAERequest
{
    ProviderSubjectNo = "COM67890",
    Company = new Company
    {
        TradeName = "ACME CORPORATION LLC",
        LegalForm = "LLC",
        RegistrationPlace = "TASHKENT",
        EconomicActivity = "62.01",
        EstablishmentDate = new DateTime(2010, 3, 15),
        Addresses = new List<Address> { /* ... */ },
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
        ContractType = "05",
        Currency = "UZS",
        FinancedAmount = 500000000
    }
};

var response = await crifService.NewApplicationEnquiryAsync(request);
```

### 3. Monitoring Enquiry

```csharp
var request = new MERequest
{
    CBSubjectCode = "CB123456",  // From NAE response
    ProductRole = ProductRole.Full,
    RequestScore = true
};

var response = await crifService.MonitoringEnquiryAsync(request);

if (response.Success && response.CreditReport != null)
{
    Console.WriteLine($"Total Contracts: {response.CreditReport.ContractHistory.AggregatedData.TotalContracts}");
    Console.WriteLine($"Credit Score: {response.Score?.ScoreValue}");
}
```

### 4. Application Update Enquiry

```csharp
var request = new AUERequest
{
    ApplicationCodes = new ApplicationCodes
    {
        CBContractCode = "CB_CONTRACT_123"
    },
    ApplicationToUpdate = new ApplicationToUpdate
    {
        FinancedAmount = 60000000,
        ContractPhase = ContractPhase.Active
    }
};

var response = await crifService.ApplicationUpdateEnquiryAsync(request);
```

### 5. Get PDF Credit Report

```csharp
var request = new NAEPRequest
{
    // Same fields as NAE
    PresentationCulture = PresentationCulture.Russian
};

var response = await crifService.NewApplicationEnquiryPresentationAsync(request);

if (response.Success && response.PresentationDocument != null)
{
    var pdfBytes = Convert.FromBase64String(response.PresentationDocument);
    await File.WriteAllBytesAsync("credit_report.pdf", pdfBytes);
}
```

## Key Features

### ✅ Type-Safe Models

All request and response models are strongly typed with proper validation.

### ✅ Multiple Enquiry Types

Supports all CRIF enquiry types: NAE, ME, AUE, PAE, NAEP, MEP.

### ✅ Individual & Company Support

Query credit history for both individuals and legal entities.

### ✅ Request Validation

Built-in validation for required fields and business rules.

### ✅ Error Handling

Comprehensive exception types for different error scenarios:
- `CrifAuthenticationException`
- `CrifCommunicationException`
- `CrifValidationException`
- `CrifConfigurationException`

### ✅ PDF Generation

NAEP/MEP enquiries return Base64-encoded PDF documents.

## Important Notes

### 🚧 SOAP Implementation Status

The SOAP client (`SoapClient.cs`) is currently a **skeleton implementation**. To complete the integration, you need to:

1. **Implement XML Serialization**: Convert C# models to CRIF XSD format
2. **Build SOAP Envelope**: Construct proper MG Request with credentials
3. **Parse SOAP Response**: Extract and deserialize MG Response to C# models
4. **Map Domain Tables**: Implement proper code mappings from CRIF documentation

Reference the CRIF XSD schemas provided in their documentation:
- `CBUzbekistan_SOAPInterface.xsd`
- `CBS.MGSOAP.MGRequest_MGResponse.xsd`

### 📋 Domain Tables Required

Many fields reference domain tables from CRIF documentation (Section 6):
- Contract types (Table 6.37)
- Economic activities (Table 6.30)
- Legal forms (Table 6.9)
- Districts & regions (Tables 6.28, 6.29)
- And many more...

### 🔐 Security

- Never commit `appsettings.local.json` with real credentials
- Use environment variables or Azure Key Vault in production
- HTTPS/TLS is required for all communication

### 💱 Currency

All amounts must be in **UZS (Uzbek Som)**. The library enforces this in configuration.

### 📅 Date Format

Dates use ISO format: `YYYY-MM-DD` (e.g., `2024-11-26`)

## Running the Examples

```bash
cd CRIF_API.Examples

# Copy and edit configuration
cp appsettings.local.json.example appsettings.local.json
# Edit appsettings.local.json with your credentials

# Run examples
dotnet run
```

## Error Codes

Common CRIF error codes (Section 7 of manual):
- `2-001` to `2-055`: Validation errors
- `2-056` to `2-061`: Data quality errors
- Authentication errors
- Communication errors

See the CRIF documentation for complete error code reference.

## API Reference

### ICrifCreditBureauService

Main service interface:

```csharp
Task<NAEResponse> NewApplicationEnquiryAsync(NAERequest, CancellationToken)
Task<PresentationResponse> NewApplicationEnquiryPresentationAsync(NAEPRequest, CancellationToken)
Task<MEResponse> MonitoringEnquiryAsync(MERequest, CancellationToken)
Task<PresentationResponse> MonitoringEnquiryPresentationAsync(MEPRequest, CancellationToken)
Task<AUEResponse> ApplicationUpdateEnquiryAsync(AUERequest, CancellationToken)
Task<PAEResponse> PortfolioAlertsEnquiryAsync(PAERequest, CancellationToken)
```

## Contributing

To complete this library:

1. Implement SOAP message building in `SoapClient.cs`
2. Add XML serialization/deserialization
3. Implement RCU (Realtime Contract Update)
4. Add comprehensive unit tests
5. Add logging infrastructure
6. Create domain table constants/enums

## Documentation References

- CRIF Manual: `CBUzbekistan_A2A_Enquiry_Manual_RU_2.1.pdf`
- Version: 2.1 (May 2024)
- XSD Schemas: Provided separately by CRIF

## License

[Your License Here]

## Support

For CRIF API support, contact your CRIF account manager.

For library issues, create an issue in the repository.
