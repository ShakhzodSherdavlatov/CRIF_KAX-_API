namespace CRIF_API.Client.Models.Responses;

/// <summary>
/// Portfolio Alerts Enquiry (PAE) Response
/// </summary>
public class PAEResponse : BaseResponse
{
    /// <summary>
    /// List of alerts
    /// </summary>
    public List<Alert> Alerts { get; set; } = new();

    /// <summary>
    /// Total alerts count
    /// </summary>
    public int TotalCount { get; set; }
}

/// <summary>
/// Portfolio alert
/// </summary>
public class Alert
{
    /// <summary>
    /// Alert code
    /// </summary>
    public string? AlertCode { get; set; }

    /// <summary>
    /// Alert description
    /// </summary>
    public string? AlertDescription { get; set; }

    /// <summary>
    /// Event date and time
    /// </summary>
    public DateTime EventDateTime { get; set; }

    /// <summary>
    /// CB Subject Code
    /// </summary>
    public string? CBSubjectCode { get; set; }

    /// <summary>
    /// CB Contract Code
    /// </summary>
    public string? CBContractCode { get; set; }

    /// <summary>
    /// Provider contract number
    /// </summary>
    public string? ProviderContractNo { get; set; }

    /// <summary>
    /// Subject name
    /// </summary>
    public string? SubjectName { get; set; }

    /// <summary>
    /// Alert details
    /// </summary>
    public string? Details { get; set; }
}
