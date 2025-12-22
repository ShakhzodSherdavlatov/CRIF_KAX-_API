# CRIF Credit Bureau API Client (C#)

A C# client library and REST API wrapper for integrating with the CRIF KAX Credit Bureau (Uzbekistan) SOAP API.

## Overview

This project provides both a **SOAP client library** and a **REST API wrapper** for the CRIF Credit Bureau system, giving you flexibility in how you integrate:

- **CRIF_API.Client** - Type-safe C# library for direct SOAP integration
- **CRIF_API.RestApi** - REST API wrapper with Swagger UI for easy HTTP-based integration

### Supported Enquiry Types

- **NAE** - New Application Enquiry
- **ME** - Monitoring Enquiry
- **AUE** - Application Update Enquiry
- **PAE** - Portfolio Alerts Enquiry
- **NAEP/MEP** - Presentation queries (with PDF output)
- **RCU** - Realtime Contract Update (TODO)

## Project Structure

```
CRIF_API/
├── CRIF_API.Client/          # SOAP client library
│   ├── Configuration/         # Settings and configuration
│   ├── Constants/            # Domain tables and constants
│   ├── Enums/                # Enumerations (ContractPhase, SubjectRole, etc.)
│   ├── Exceptions/           # Custom exceptions
│   ├── Models/               # DTOs
│   │   ├── Common/          # Shared models (Individual, Company, Address, etc.)
│   │   ├── Requests/        # SOAP request models
│   │   └── Responses/       # SOAP response models
│   └── Services/            # SOAP service interfaces and implementations
├── CRIF_API.RestApi/         # REST API wrapper
│   ├── Controllers/          # REST API controllers
│   ├── Models/               # REST API request/response models
│   ├── appsettings.json      # Configuration with test credentials
│   └── example-request.json  # Example REST API request
└── CRIF_API.Examples/        # Example console application
```

## Prerequisites

- .NET 9.0 or higher
- CRIF Credit Bureau credentials (User ID & Password)
- Access to CRIF UAT or Production endpoint

## Quick Start

### Option 1: Use the REST API (Recommended for most integrations)

The REST API provides a simple HTTP interface with Swagger documentation:

```bash
cd CRIF_API.RestApi
dotnet run
```

Navigate to `https://localhost:5001` to access Swagger UI, where you can:
- View all available endpoints
- Test API calls directly in the browser
- See request/response schemas
- Use the `/api/CreditBureau/test` endpoint with pre-filled test data

### Option 2: Use the Client Library Directly

Install and reference the CRIF_API.Client library in your C# project for direct SOAP integration.

## Test Credentials (UAT Environment)

The project includes pre-configured test credentials for the CRIF UAT environment:

- **Endpoint**: `https://a2atest.crifkax.uz/Service.svc`
- **User ID**: `MUHE4219`
- **Password**: `621VrYt+!o`

These credentials are configured in `CRIF_API.RestApi/appsettings.json` and are safe for testing purposes. For production, replace these with your own credentials from CRIF.

**Note**: Never commit production credentials to version control. Use environment variables or secure configuration management.

## Installation

### Option 1: Add as Project Reference

```bash
dotnet add reference path/to/CRIF_API.Client/CRIF_API.Client.csproj
```

### Option 2: Build and Use DLL

```bash
cd CRIF_API.Client
dotnet build -c Release
# Reference the DLL from bin/Release/net9.0/
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

## Usage Examples

## REST API Examples

### 1. Create Application Enquiry (Using cURL)

```bash
curl -X POST "https://localhost:5001/api/CreditBureau/applications" \
  -H "Content-Type: application/json" \
  -d @example-request.json
```

### 2. REST API Request Example (Individual)

```json
{
  "providerSubjectNo": "IND12345",
  "subjectRefDate": "2024-12-19",
  "individual": {
    "firstName": "JOHN",
    "lastName": "DOE",
    "middleName": "SMITH",
    "dateOfBirth": "1990-05-15",
    "gender": "M",
    "pinfl": "12345678901234",
    "documents": [
      {
        "documentType": "1",
        "documentNumber": "AB1234567",
        "issueDate": "2020-01-15",
        "expirationDate": "2030-01-15",
        "issuingAuthority": "OVIR Tashkent"
      }
    ],
    "address": {
      "city": "TASHKENT",
      "district": "UCHTEPA",
      "region": "TASHKENT_CITY",
      "street": "AMIR TEMUR",
      "building": "12",
      "apartment": "34",
      "country": "UZ"
    },
    "phoneNumbers": ["+998901234567"]
  },
  "application": {
    "contractType": "01",
    "contractPhase": "Requested",
    "contractRequestDate": "2024-12-19",
    "currency": "UZS",
    "financedAmount": 50000000,
    "monthlyPaymentAmount": 5000000,
    "installmentsNumber": 12,
    "paymentPeriodicity": "M",
    "subjectRole": "Borrower"
  },
  "providerContractNo": "LOAN-2024-001"
}
```

### 3. REST API Response Example

```json
{
  "success": true,
  "cbContractCode": "CB_CONTRACT_123456",
  "cbSubjectCode": "CB_SUBJECT_789012",
  "subjectMatched": true,
  "creditReport": {
    "totalActiveContracts": 3,
    "totalDebtAmount": 15000000,
    "overdueContracts": 0,
    "creditScore": 750,
    "riskClass": "Low Risk"
  }
}
```

### 4. Test Endpoint (Pre-filled Data)

```bash
curl -X POST "https://localhost:5001/api/CreditBureau/test"
```

This endpoint uses pre-filled test data from the CRIF documentation for quick testing.

### 5. Health Check

```bash
curl "https://localhost:5001/api/CreditBureau/health"
```

## C# Client Library Examples

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

### ✅ Dual Integration Options

Choose between REST API for simplicity or C# library for advanced control.

### ✅ REST API with Swagger UI

Interactive API documentation and testing interface included.

### ✅ Type-Safe Models

All request and response models are strongly typed with proper validation.

### ✅ Multiple Enquiry Types

Supports all CRIF enquiry types: NAE, ME, AUE, PAE, NAEP, MEP.

### ✅ Individual & Company Support

Query credit history for both individuals and legal entities.

### ✅ Request Validation

Built-in validation for required fields and business rules.

### ✅ Comprehensive Error Handling

REST API returns standardized error responses with proper HTTP status codes.
Client library provides specific exception types:
- `CrifAuthenticationException`
- `CrifCommunicationException`
- `CrifValidationException`
- `CrifConfigurationException`

### ✅ PDF Generation

NAEP/MEP enquiries return Base64-encoded PDF documents.

### ✅ Test Credentials Included

Pre-configured test credentials for the CRIF UAT environment.

### ✅ Production-Ready

Complete SOAP implementation with XML serialization, envelope building, and response parsing.

## Important Notes

### 🔧 REST API Endpoints

The REST API currently supports:
- `POST /api/CreditBureau/applications` - Create new application enquiry
- `POST /api/CreditBureau/test` - Test with pre-filled data
- `GET /api/CreditBureau/health` - Health check

Additional endpoints for ME, AUE, PAE operations can be added following the same pattern.

### ⚠️ Mandatory Fields

The following fields are **required** for New Application Enquiry (NAE):

**For Individual**:
- `FirstName` and `LastName`
- `SubjectRefDate` - Date to which subject data refers
- `ContractRequestDate` - Date when application was requested
- At least one identification code (PINFL or TIN recommended)
- At least one address with City, Region, and Country
- `ContractType` - Type of loan/credit product
- `Currency` - Must be "UZS"

**For Company**:
- `TradeName` (company name)
- `SubjectRefDate`
- `ContractRequestDate`
- At least one identification code (TIN required)
- At least one address
- `ContractType`
- `Currency` - Must be "UZS"

**For Credit Card contracts** (ContractType = "53"):
- Use `CreditLimit` instead of `FinancedAmount`

**For Installment contracts**:
- `FinancedAmount` is required
- `InstallmentsNumber` and `PaymentPeriodicity` are recommended

### 📋 Domain Tables & Code Mappings

Many fields use codes from CRIF domain tables (Documentation Section 6):
- **Contract types** (Table 6.37): "01" = Consumer Loan, "53" = Credit Card, etc.
- **Document types**: "1" = ID Card, "2" = Passport, "3" = Permanent Residence, etc.
- **Economic activities** (Table 6.30): NACE classification codes
- **Legal forms** (Table 6.9): LLC, JSC, Individual Entrepreneur, etc.
- **Districts & Regions** (Tables 6.28, 6.29): Uzbekistan administrative divisions
- **Contact types**: "1" = Mobile, "2" = Home, "3" = Work, etc.
- **Occupation status**: "1" = Employed, "2" = Self-employed, etc.

Refer to the CRIF documentation for complete code lists.

### 🔐 Security

- Test credentials (UAT) are included in `appsettings.json` for convenience
- **Never** commit production credentials to version control
- For production, use environment variables, Azure Key Vault, or secure configuration management
- HTTPS/TLS is required for all communication
- Rotate credentials periodically and follow your organization's security policies

### 💱 Currency

All amounts must be in **UZS (Uzbek Som)**. The library enforces this in configuration.

### 📅 Date Format

Dates use ISO format: `YYYY-MM-DD` (e.g., `2024-11-26`)

## Running the Project

### Run the REST API

```bash
cd CRIF_API.RestApi
dotnet run

# Access Swagger UI at: https://localhost:5001
# API Base URL: https://localhost:5001/api/CreditBureau
```

The REST API includes test credentials in `appsettings.json` for the UAT environment.

### Run the Console Examples

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

### REST API Endpoints

Base URL: `https://localhost:5001/api/CreditBureau`

#### POST /applications
Create a new credit application enquiry.

**Request Body**: `NewApplicationRequest` (see example-request.json)

**Response**: `NewApplicationResponse`
```json
{
  "success": true,
  "cbContractCode": "string",
  "cbSubjectCode": "string",
  "subjectMatched": true,
  "creditReport": {
    "totalActiveContracts": 0,
    "totalDebtAmount": 0,
    "overdueContracts": 0,
    "creditScore": 0,
    "riskClass": "string"
  },
  "errorCode": "string",
  "errorMessage": "string"
}
```

#### POST /test
Test endpoint with pre-filled CRIF documentation example data.

**Response**: Same as /applications

#### GET /health
Health check endpoint.

**Response**:
```json
{
  "status": "healthy",
  "timestamp": "2024-12-19T10:00:00Z"
}
```

### C# Client Library - ICrifCreditBureauService

Main service interface for direct SOAP integration:

```csharp
Task<NAEResponse> NewApplicationEnquiryAsync(NAERequest, CancellationToken)
Task<PresentationResponse> NewApplicationEnquiryPresentationAsync(NAEPRequest, CancellationToken)
Task<MEResponse> MonitoringEnquiryAsync(MERequest, CancellationToken)
Task<PresentationResponse> MonitoringEnquiryPresentationAsync(MEPRequest, CancellationToken)
Task<AUEResponse> ApplicationUpdateEnquiryAsync(AUERequest, CancellationToken)
Task<PAEResponse> PortfolioAlertsEnquiryAsync(PAERequest, CancellationToken)
```

## Contributing

Potential enhancements:

1. Add additional REST API endpoints for ME, AUE, PAE operations
2. Implement RCU (Realtime Contract Update) enquiry type
3. Add comprehensive unit and integration tests
4. Enhance error handling and retry logic
5. Add request/response logging and auditing
6. Create NuGet package for the client library
7. Add more domain table constants and validation
8. Implement caching for frequently accessed data
9. Add batch processing capabilities
10. Create additional language bindings (Python, JavaScript, etc.)

## REST API vs. Client Library

### When to use REST API

✅ **Best for**:
- Quick integration without C# knowledge
- Microservices architecture
- Language-agnostic clients (JavaScript, Python, etc.)
- Testing and prototyping with Swagger UI
- Simplified request/response models

### When to use Client Library

✅ **Best for**:
- Full control over SOAP communication
- Advanced C# applications
- Direct access to all CRIF response fields
- Type-safe compile-time validation
- Custom error handling and retry logic
- Performance-critical applications (no REST overhead)

## Documentation References

- CRIF Manual: `CBUzbekistan_A2A_Enquiry_Manual_RU_2.1.pdf`
- Version: 2.1 (May 2024)
- XSD Schemas: Provided separately by CRIF

## License

[Your License Here]

## Support

For CRIF API support, contact your CRIF account manager.

For library issues, create an issue in the repository.
