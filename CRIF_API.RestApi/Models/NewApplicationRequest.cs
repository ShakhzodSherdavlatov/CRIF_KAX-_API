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
    public string? Gender { get; set; }

    /// <summary>
    /// PINFL (Personal Identification Number)
    /// </summary>
    public string? PINFL { get; set; }

    /// <summary>
    /// Passport or ID document
    /// </summary>
    public DocumentData? Document { get; set; }

    /// <summary>
    /// Address information
    /// </summary>
    public AddressData? Address { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    public string? PhoneNumber { get; set; }
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
    /// Currency (must be UZS)
    /// </summary>
    public string Currency { get; set; } = "UZS";

    /// <summary>
    /// Loan amount
    /// </summary>
    public decimal FinancedAmount { get; set; }

    /// <summary>
    /// Monthly payment amount
    /// </summary>
    public decimal? MonthlyPaymentAmount { get; set; }

    /// <summary>
    /// Loan term in months
    /// </summary>
    public int? DurationInMonths { get; set; }

    /// <summary>
    /// Subject role: "Borrower", "CoApplicant", "Guarantor"
    /// </summary>
    public string SubjectRole { get; set; } = "Borrower";
}
