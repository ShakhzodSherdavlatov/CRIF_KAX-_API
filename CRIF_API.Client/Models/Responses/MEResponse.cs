using CRIF_API.Client.Models.Common;

namespace CRIF_API.Client.Models.Responses;

/// <summary>
/// Monitoring Enquiry (ME) Response
/// </summary>
public class MEResponse : BaseResponse
{
    /// <summary>
    /// Enquired data (echo of request)
    /// </summary>
    public EnquiredData? EnquiredData { get; set; }

    /// <summary>
    /// Credit report
    /// </summary>
    public CreditReport? CreditReport { get; set; }

    /// <summary>
    /// Credit score (if requested)
    /// </summary>
    public CreditScore? Score { get; set; }
}
