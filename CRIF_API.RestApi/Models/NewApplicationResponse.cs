namespace CRIF_API.RestApi.Models;

/// <summary>
/// REST API response for new application enquiry
/// </summary>
public class NewApplicationResponse
{
    /// <summary>
    /// Request was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Error code from CRIF
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// CRIF's contract code
    /// </summary>
    public string? CBContractCode { get; set; }

    /// <summary>
    /// CRIF's subject code
    /// </summary>
    public string? CBSubjectCode { get; set; }

    /// <summary>
    /// Was subject matched in CRIF database
    /// </summary>
    public bool? SubjectMatched { get; set; }

    /// <summary>
    /// Credit report summary (optional)
    /// </summary>
    public CreditReportSummary? CreditReport { get; set; }
}

/// <summary>
/// Summary of credit report
/// </summary>
public class CreditReportSummary
{
    /// <summary>
    /// Total number of active contracts
    /// </summary>
    public int? TotalActiveContracts { get; set; }

    /// <summary>
    /// Total debt amount
    /// </summary>
    public decimal? TotalDebtAmount { get; set; }

    /// <summary>
    /// Number of overdue contracts
    /// </summary>
    public int? OverdueContracts { get; set; }

    /// <summary>
    /// Credit score (if available)
    /// </summary>
    public int? CreditScore { get; set; }

    /// <summary>
    /// Risk classification
    /// </summary>
    public string? RiskClass { get; set; }
}
