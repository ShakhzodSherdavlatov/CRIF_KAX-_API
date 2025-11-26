namespace CRIF_API.Client.Models.Responses;

/// <summary>
/// Application Update Enquiry (AUE) Response
/// </summary>
public class AUEResponse : BaseResponse
{
    /// <summary>
    /// Application codes
    /// </summary>
    public ApplicationCodes? ApplicationCodes { get; set; }

    /// <summary>
    /// Application data from DB (before update)
    /// </summary>
    public ApplicationSnapshot? ApplicationDB { get; set; }

    /// <summary>
    /// Updated application data (after update)
    /// </summary>
    public ApplicationSnapshot? ApplicationUpdated { get; set; }
}

/// <summary>
/// Application snapshot
/// </summary>
public class ApplicationSnapshot
{
    public string? ContractType { get; set; }
    public string? ContractPhase { get; set; }
    public DateTime? ContractRequestDate { get; set; }
    public decimal? FinancedAmount { get; set; }
    public decimal? CreditLimit { get; set; }
    public decimal? MonthlyPaymentAmount { get; set; }
    public bool? CancellationFlag { get; set; }
}
