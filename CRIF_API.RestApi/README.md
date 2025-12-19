# CRIF Credit Bureau REST API

This is a REST API wrapper for the CRIF Credit Bureau SOAP service. It provides JSON-based endpoints for easy integration with other services.

## Running the API

```bash
cd CRIF_API.RestApi
dotnet run
```

The API will start at `https://localhost:5001` (HTTPS) and `http://localhost:5000` (HTTP).

Swagger UI will be available at: `https://localhost:5001/`

## Configuration

Update `appsettings.json` with your CRIF credentials:

```json
{
  "CrifSettings": {
    "EndpointUrl": "https://a2atest.crifkax.uz/Service.svc",
    "UserId": "MUHE4219",
    "Password": "621VrYt+!o",
    "TimeoutSeconds": 120,
    "Environment": "UAT",
    "EnableLogging": true
  }
}
```

## API Endpoints

### 1. Create New Application (Individual)

**POST** `/api/CreditBureau/applications`

**Request Body:**
```json
{
  "providerSubjectNo": "IND12345",
  "individual": {
    "firstName": "JOHN",
    "lastName": "DOE",
    "middleName": "SMITH",
    "dateOfBirth": "1990-05-15",
    "gender": "M",
    "pinfl": "12345678901234",
    "document": {
      "documentType": "PASSPORT",
      "documentNumber": "AB1234567",
      "issueDate": "2020-01-15",
      "expirationDate": "2030-01-15"
    },
    "address": {
      "city": "TASHKENT",
      "district": "UCHTEPA",
      "region": "TASHKENT_CITY",
      "street": "AMIR TEMUR",
      "building": "12",
      "apartment": "34"
    },
    "phoneNumber": "+998901234567"
  },
  "application": {
    "contractType": "01",
    "contractPhase": "Requested",
    "currency": "UZS",
    "financedAmount": 50000000,
    "monthlyPaymentAmount": 5000000,
    "subjectRole": "Borrower"
  },
  "providerContractNo": "LOAN-2024-001"
}
```

**Response:**
```json
{
  "success": true,
  "cbContractCode": "CB_CONTRACT_123",
  "cbSubjectCode": "CB_SUBJECT_456",
  "subjectMatched": true,
  "creditReport": {
    "totalActiveContracts": 2,
    "totalDebtAmount": 15000000,
    "overdueContracts": 0,
    "creditScore": 750
  }
}
```

### 2. Create New Application (Company)

**POST** `/api/CreditBureau/applications`

**Request Body:**
```json
{
  "providerSubjectNo": "COM67890",
  "company": {
    "tradeName": "ACME CORPORATION LLC",
    "legalForm": "LLC",
    "registrationPlace": "TASHKENT",
    "economicActivity": "62.01",
    "establishmentDate": "2010-03-15",
    "tin": "123456789",
    "address": {
      "city": "TASHKENT",
      "district": "YAKKASARAY",
      "region": "TASHKENT_CITY",
      "street": "AMIR TEMUR",
      "building": "100"
    }
  },
  "application": {
    "contractType": "05",
    "contractPhase": "Requested",
    "currency": "UZS",
    "financedAmount": 500000000,
    "subjectRole": "Borrower"
  },
  "providerContractNo": "CORP-LOAN-2024-001"
}
```

### 3. Health Check

**GET** `/api/CreditBureau/health`

**Response:**
```json
{
  "status": "healthy",
  "timestamp": "2024-12-18T10:30:00Z"
}
```

## Testing with cURL

### Create Individual Application:
```bash
curl -X POST "https://localhost:5001/api/CreditBureau/applications" \
  -H "Content-Type: application/json" \
  -d @example-request.json
```

### Health Check:
```bash
curl "https://localhost:5001/api/CreditBureau/health"
```

## Testing with PowerShell

```powershell
$body = Get-Content -Path "example-request.json" -Raw

$response = Invoke-RestMethod `
  -Uri "https://localhost:5001/api/CreditBureau/applications" `
  -Method POST `
  -ContentType "application/json" `
  -Body $body

$response | ConvertTo-Json -Depth 10
```

## Contract Types

Common contract types (from CRIF domain tables):
- `01` - Consumer loan
- `02` - Mortgage
- `03` - Car loan
- `04` - Micro loan
- `05` - Business loan
- `06` - Credit card
- `07` - Overdraft

## Subject Roles

- `Borrower` - Primary borrower
- `CoBorrower` - Co-borrower (multiple parties)
- `Guarantor` - Guarantor

## Error Handling

Error responses include:
```json
{
  "success": false,
  "message": "Error description",
  "errorCode": "ERROR_CODE",
  "validationErrors": {
    "field1": ["error1", "error2"]
  }
}
```

Common error codes:
- `VALIDATION_ERROR` - Input validation failed
- `AUTH_ERROR` - CRIF authentication failed
- `COMMUNICATION_ERROR` - Network/connection error
- `INTERNAL_ERROR` - Unexpected server error

## Notes

- All amounts must be in UZS (Uzbek Som)
- Dates use ISO format: `YYYY-MM-DD`
- The API wraps the CRIF SOAP service, so all SOAP XML handling is done internally
- CRIF credentials are stored securely in `appsettings.json` (use Azure Key Vault or similar in production)
- Logging can be enabled via `EnableLogging` in configuration

## Architecture

```
REST Client → REST API (JSON) → CreditBureauController →
CRIF Service → SOAP Client → CRIF SOAP API (XML)
```

The REST API acts as a facade, converting JSON requests to SOAP XML and vice versa.
