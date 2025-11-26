namespace CRIF_API.Client.Models.Requests;

/// <summary>
/// Portfolio Alerts Enquiry (PAE) Request
/// Used to retrieve portfolio alerts
/// </summary>
public class PAERequest
{
    /// <summary>
    /// Date from (filter alerts from this date)
    /// </summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>
    /// Date to (filter alerts until this date)
    /// </summary>
    public DateTime? DateTo { get; set; }

    /// <summary>
    /// Alert code filter (optional) - see domain tables
    /// </summary>
    public string? AlertCode { get; set; }

    /// <summary>
    /// Maximum number of alerts to retrieve
    /// </summary>
    public int? MaxResults { get; set; }
}
