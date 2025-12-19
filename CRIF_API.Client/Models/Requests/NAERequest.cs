using CRIF_API.Client.Enums;
using CRIF_API.Client.Models.Common;

namespace CRIF_API.Client.Models.Requests;

/// <summary>
/// New Application Enquiry (NAE) Request
/// Used when provider receives a new credit application from a subject
/// </summary>
public class NAERequest
{
    /// <summary>
    /// Provider subject number (unique ID assigned by provider)
    /// </summary>
    public string? ProviderSubjectNo { get; set; }

    /// <summary>
    /// Subject reference date (MANDATORY) - date to which subject data refers
    /// </summary>
    public DateTime SubjectRefDate { get; set; } = DateTime.Now.Date;

    /// <summary>
    /// Individual data (required if subject is individual)
    /// </summary>
    public Individual? Individual { get; set; }

    /// <summary>
    /// Company data (required if subject is company)
    /// </summary>
    public Company? Company { get; set; }

    /// <summary>
    /// Application data
    /// </summary>
    public ApplicationData? Application { get; set; }

    /// <summary>
    /// Link/Role information
    /// </summary>
    public LinkData Link { get; set; } = new();

    /// <summary>
    /// Application codes
    /// </summary>
    public ApplicationCodes ApplicationCodes { get; set; } = new();

    /// <summary>
    /// Product role (Full, Basic, ApplicationOnly)
    /// </summary>
    public ProductRole ProductRole { get; set; } = ProductRole.Full;

    /// <summary>
    /// Request credit score
    /// </summary>
    public bool RequestScore { get; set; } = false;
}

/// <summary>
/// Application data for new credit application
/// </summary>
public class ApplicationData
{
    /// <summary>
    /// Contract type code (required) - see domain tables
    /// </summary>
    public string ContractType { get; set; } = string.Empty;

    /// <summary>
    /// Contract phase (required)
    /// </summary>
    public ContractPhase ContractPhase { get; set; } = ContractPhase.Requested;

    /// <summary>
    /// Contract request date (required)
    /// Default: current date, can be backdated
    /// </summary>
    public DateTime ContractRequestDate { get; set; } = DateTime.Now.Date;

    /// <summary>
    /// Currency (always UZS)
    /// </summary>
    public string Currency { get; set; } = "UZS";

    /// <summary>
    /// Financed amount (for installment contracts)
    /// </summary>
    public decimal? FinancedAmount { get; set; }

    /// <summary>
    /// Credit limit (for credit cards and revolving credit)
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
    /// Payment periodicity code (required for installment contracts)
    /// Default: "M" = Monthly (30 days)
    /// </summary>
    public string PaymentPeriodicity { get; set; } = "M";

    /// <summary>
    /// Due date (expected contract end date)
    /// </summary>
    public DateTime? DueDate { get; set; }
}

/// <summary>
/// Link data - role of subject in contract
/// </summary>
public class LinkData
{
    /// <summary>
    /// Role of subject (required)
    /// </summary>
    public SubjectRole RoleOfSubject { get; set; } = SubjectRole.Borrower;

    /// <summary>
    /// Company role (for individuals linked to company contracts)
    /// </summary>
    public CompanyRole? CompanyRole { get; set; }
}

/// <summary>
/// Application codes for identifying the contract/application
/// </summary>
public class ApplicationCodes
{
    /// <summary>
    /// Provider contract number (if using same number for application and contract)
    /// </summary>
    public string? ProviderContractNo { get; set; }

    /// <summary>
    /// Provider application number (if using temporary number for application)
    /// </summary>
    public string? ProviderApplicationNo { get; set; }

    /// <summary>
    /// CB contract code (returned by CB, used for linking additional subjects)
    /// </summary>
    public string? CBContractCode { get; set; }
}
