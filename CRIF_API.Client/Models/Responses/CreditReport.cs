using CRIF_API.Client.Enums;
using CRIF_API.Client.Models.Common;

namespace CRIF_API.Client.Models.Responses;

/// <summary>
/// Credit Report - main response data
/// </summary>
public class CreditReport
{
    /// <summary>
    /// Matched subject information
    /// </summary>
    public MatchedSubject? MatchedSubject { get; set; }

    /// <summary>
    /// Contract history
    /// </summary>
    public ContractHistory? ContractHistory { get; set; }

    /// <summary>
    /// Enquiry footprint (query history)
    /// </summary>
    public Footprint? Footprint { get; set; }
}

/// <summary>
/// Matched subject from CB database
/// </summary>
public class MatchedSubject
{
    /// <summary>
    /// CB Subject Code (unique ID assigned by CB)
    /// </summary>
    public string? CBSubjectCode { get; set; }

    /// <summary>
    /// Flag indicating if match was found
    /// </summary>
    public bool FlagMatched { get; set; }

    /// <summary>
    /// Individual data (if subject type is individual)
    /// </summary>
    public Individual? Individual { get; set; }

    /// <summary>
    /// Company data (if subject type is company)
    /// </summary>
    public Company? Company { get; set; }
}

/// <summary>
/// Contract history with aggregated and detailed data
/// </summary>
public class ContractHistory
{
    /// <summary>
    /// Aggregated data summary
    /// </summary>
    public AggregatedData? AggregatedData { get; set; }

    /// <summary>
    /// Not granted contracts (requested, refused, renounced)
    /// </summary>
    public List<NotGrantedContract> NotGrantedContracts { get; set; } = new();

    /// <summary>
    /// Granted contracts (active, closed, closed in advance)
    /// </summary>
    public List<GrantedContract> GrantedContracts { get; set; } = new();
}

/// <summary>
/// Aggregated contract data
/// </summary>
public class AggregatedData
{
    /// <summary>
    /// Total number of contracts
    /// </summary>
    public int TotalContracts { get; set; }

    /// <summary>
    /// Total number of providers
    /// </summary>
    public int TotalProviders { get; set; }

    /// <summary>
    /// Currency (always UZS)
    /// </summary>
    public string Currency { get; set; } = "UZS";

    /// <summary>
    /// Numbers summary (grouped by category and phase)
    /// </summary>
    public NumbersSummary? NumbersSummary { get; set; }

    /// <summary>
    /// Amounts summary (key values for active contracts)
    /// </summary>
    public AmountsSummary? AmountsSummary { get; set; }
}

/// <summary>
/// Numbers summary - contract counts by category and phase
/// </summary>
public class NumbersSummary
{
    /// <summary>
    /// Count by contract category and phase (matrix)
    /// Key: category-phase, Value: count
    /// </summary>
    public Dictionary<string, int> CountsByCategory { get; set; } = new();
}

/// <summary>
/// Amounts summary - financial data for active contracts
/// </summary>
public class AmountsSummary
{
    /// <summary>
    /// Total contracts count
    /// </summary>
    public int ContractsCount { get; set; }

    /// <summary>
    /// Total credit limit
    /// </summary>
    public decimal TotalCreditLimit { get; set; }

    /// <summary>
    /// Total debit balance (current outstanding)
    /// </summary>
    public decimal TotalDebitBalance { get; set; }

    /// <summary>
    /// Total overdue amount
    /// </summary>
    public decimal TotalOverdueAmount { get; set; }

    /// <summary>
    /// Amounts by contract category and role
    /// </summary>
    public List<CategoryAmounts> AmountsByCategory { get; set; } = new();
}

/// <summary>
/// Amounts grouped by category
/// </summary>
public class CategoryAmounts
{
    public ContractCategory Category { get; set; }
    public SubjectRole Role { get; set; }
    public int ContractsCount { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal DebitBalance { get; set; }
    public decimal OverdueAmount { get; set; }
}

/// <summary>
/// Not granted contract (application)
/// </summary>
public class NotGrantedContract
{
    public string? CBContractCode { get; set; }
    public string? ProviderContractNo { get; set; }
    public string? ContractType { get; set; }
    public ContractPhase? ContractPhase { get; set; }
    public SubjectRole? RoleOfSubject { get; set; }
    public DateTime? ContractRequestDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? LastUpdateDate { get; set; }
    public string? ProviderCode { get; set; }
    public string? ProviderName { get; set; }
    public List<LinkedSubject> LinkedSubjects { get; set; } = new();
    public string? Note { get; set; }
}

/// <summary>
/// Granted contract (active or closed)
/// </summary>
public class GrantedContract
{
    public string? CBContractCode { get; set; }
    public string? ProviderContractNo { get; set; }
    public string? ContractType { get; set; }
    public ContractPhase? ContractPhase { get; set; }
    public SubjectRole? RoleOfSubject { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public string? OriginalCurrency { get; set; }
    public DateTime? LastUpdateDate { get; set; }
    public string? ProviderCode { get; set; }
    public string? ProviderName { get; set; }

    // Maximum values (historical)
    public decimal? MaxOverduePaymentsAmount { get; set; }
    public int? MaxOverduePaymentsNumber { get; set; }
    public DateTime? MaxOverduePaymentsNumberDate { get; set; }
    public int? MaxDaysPastDue { get; set; }
    public DateTime? MaxDaysPastDueDate { get; set; }
    public string? WorstStatus { get; set; }
    public DateTime? WorstStatusDate { get; set; }

    // Payment history (last 24 months)
    public List<PaymentHistory> PaymentHistory { get; set; } = new();

    public List<LinkedSubject> LinkedSubjects { get; set; } = new();
    public string? Note { get; set; }
}

/// <summary>
/// Linked subject in contract
/// </summary>
public class LinkedSubject
{
    public SubjectRole? RoleOfSubject { get; set; }
    public string? Name { get; set; }
    public string? CBSubjectCode { get; set; }
}

/// <summary>
/// Payment history record (monthly snapshot)
/// </summary>
public class PaymentHistory
{
    public int ReferenceYear { get; set; }
    public int ReferenceMonth { get; set; }
    public decimal OutstandingBalance { get; set; }
    public int? OverduePaymentsNumber { get; set; }
    public int? DaysPastDue { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// Enquiry footprint - query history
/// </summary>
public class Footprint
{
    public FootprintCounters? Counters { get; set; }
    public List<FootprintData> Data { get; set; } = new();
}

/// <summary>
/// Footprint counters
/// </summary>
public class FootprintCounters
{
    public int Count1Month { get; set; }
    public int Count3Months { get; set; }
    public int Count6Months { get; set; }
    public int Count12Months { get; set; }
}

/// <summary>
/// Individual footprint data
/// </summary>
public class FootprintData
{
    public EnquiryType EnquiryType { get; set; }
    public DateTime EnquiryDate { get; set; }
    public string? InstituteName { get; set; }
}
