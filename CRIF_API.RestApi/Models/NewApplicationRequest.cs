namespace CRIF_API.RestApi.Models;

/// <summary>
/// REST API request for creating a new credit application
/// </summary>
public class NewApplicationRequest
{
    /// <summary>
    /// Provider's internal subject number
    /// </summary>
    public string? ProviderSubjectNo { get; set; }

    /// <summary>
    /// Date to which the subject data refers (MANDATORY)
    /// </summary>
    public DateTime SubjectRefDate { get; set; } = DateTime.Today;

    /// <summary>
    /// Individual person data (required if Company is null)
    /// </summary>
    public IndividualData? Individual { get; set; }

    /// <summary>
    /// Company data (required if Individual is null)
    /// </summary>
    public CompanyData? Company { get; set; }

    /// <summary>
    /// Application/loan details
    /// </summary>
    public ApplicationInfo Application { get; set; } = new();

    /// <summary>
    /// Provider's contract number
    /// </summary>
    public string? ProviderContractNo { get; set; }
}

/// <summary>
/// Individual person information
/// </summary>
public class IndividualData
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? BirthPlace { get; set; }
    public string? Gender { get; set; }

    /// <summary>
    /// PINFL (Personal Identification Number)
    /// </summary>
    public string? PINFL { get; set; }

    /// <summary>
    /// TIN (Tax Identification Number for individuals)
    /// </summary>
    public string? TIN { get; set; }

    /// <summary>
    /// Passport or ID documents (can have multiple)
    /// </summary>
    public List<DocumentData>? Documents { get; set; }

    /// <summary>
    /// Address information
    /// </summary>
    public AddressData? Address { get; set; }

    /// <summary>
    /// Phone numbers (can have multiple)
    /// </summary>
    public List<string>? PhoneNumbers { get; set; }

    /// <summary>
    /// Employment information
    /// </summary>
    public EmploymentData? Employment { get; set; }
}

/// <summary>
/// Company information
/// </summary>
public class CompanyData
{
    public string TradeName { get; set; } = string.Empty;
    public string? LegalForm { get; set; }
    public string? RegistrationPlace { get; set; }
    public string? EconomicActivity { get; set; }
    public DateTime? EstablishmentDate { get; set; }

    /// <summary>
    /// TIN (Tax Identification Number)
    /// </summary>
    public string? TIN { get; set; }

    /// <summary>
    /// Company address
    /// </summary>
    public AddressData? Address { get; set; }
}

/// <summary>
/// Document information
/// </summary>
public class DocumentData
{
    public string? DocumentType { get; set; }
    public string? DocumentNumber { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? IssuingAuthority { get; set; }
}

/// <summary>
/// Address information
/// </summary>
public class AddressData
{
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Region { get; set; }
    public string? Street { get; set; }
    public string? Building { get; set; }
    public string? Apartment { get; set; }
    public string Country { get; set; } = "UZ";
}

/// <summary>
/// Application/loan information
/// </summary>
public class ApplicationInfo
{
    /// <summary>
    /// Contract type code (e.g., "01" for consumer loan)
    /// </summary>
    public string ContractType { get; set; } = string.Empty;

    /// <summary>
    /// Contract phase: "Requested", "Active", etc.
    /// </summary>
    public string ContractPhase { get; set; } = "Requested";

    /// <summary>
    /// Date when the credit application was requested (MANDATORY)
    /// </summary>
    public DateTime ContractRequestDate { get; set; } = DateTime.Today;

    /// <summary>
    /// Currency (must be UZS)
    /// </summary>
    public string Currency { get; set; } = "UZS";

    /// <summary>
    /// Loan amount (for installment contracts)
    /// </summary>
    public decimal? FinancedAmount { get; set; }

    /// <summary>
    /// Credit limit (for credit card contracts)
    /// </summary>
    public decimal? CreditLimit { get; set; }

    /// <summary>
    /// Monthly payment amount
    /// </summary>
    public decimal? MonthlyPaymentAmount { get; set; }

    /// <summary>
    /// Number of installments (required for installment contracts)
    /// </summary>
    public int? InstallmentsNumber { get; set; }

    /// <summary>
    /// Payment periodicity code: "M" = Monthly, "Q" = Quarterly, "S" = Semi-Annual, "Y" = Yearly (required for installment contracts)
    /// </summary>
    public string PaymentPeriodicity { get; set; } = "M";

    /// <summary>
    /// Subject role: "Borrower", "CoApplicant", "Guarantor"
    /// </summary>
    public string SubjectRole { get; set; } = "Borrower";
}

/// <summary>
/// Employment information
/// </summary>
public class EmploymentData
{
    public decimal? GrossIncome { get; set; }
    public string Currency { get; set; } = "UZS";
    public string? OccupationStatus { get; set; }
    public DateTime? DateHiredFrom { get; set; }
    public string? Occupation { get; set; }
    public string? EmployerName { get; set; }
}
