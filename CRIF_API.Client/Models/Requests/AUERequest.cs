using CRIF_API.Client.Enums;

namespace CRIF_API.Client.Models.Requests;

/// <summary>
/// Application Update Enquiry (AUE) Request
/// Used to update previously submitted applications
/// </summary>
public class AUERequest
{
    /// <summary>
    /// Application data to update
    /// </summary>
    public ApplicationToUpdate ApplicationToUpdate { get; set; } = new();

    /// <summary>
    /// Application codes (must provide one)
    /// </summary>
    public ApplicationCodes ApplicationCodes { get; set; } = new();
}

/// <summary>
/// Application update data
/// </summary>
public class ApplicationToUpdate
{
    /// <summary>
    /// Contract phase
    /// </summary>
    public ContractPhase? ContractPhase { get; set; }

    /// <summary>
    /// Cancellation flag (to cancel application submitted by error)
    /// </summary>
    public bool? CancellationFlag { get; set; }

    /// <summary>
    /// Financed amount
    /// </summary>
    public decimal? FinancedAmount { get; set; }

    /// <summary>
    /// Credit limit
    /// </summary>
    public decimal? CreditLimit { get; set; }

    /// <summary>
    /// Monthly payment amount
    /// </summary>
    public decimal? MonthlyPaymentAmount { get; set; }
}
