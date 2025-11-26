using CRIF_API.Client.Enums;
using CRIF_API.Client.Models.Common;

namespace CRIF_API.Client.Models.Requests;

/// <summary>
/// Monitoring Enquiry (ME) Request
/// Used to monitor existing customers' credit status
/// </summary>
public class MERequest
{
    /// <summary>
    /// CB Subject Code (returned from NAE)
    /// </summary>
    public string? CBSubjectCode { get; set; }

    /// <summary>
    /// Provider Subject Number
    /// </summary>
    public string? ProviderSubjectNo { get; set; }

    /// <summary>
    /// Individual data (optional if using codes)
    /// </summary>
    public Individual? Individual { get; set; }

    /// <summary>
    /// Company data (optional if using codes)
    /// </summary>
    public Company? Company { get; set; }

    /// <summary>
    /// Product role (Full or Basic)
    /// </summary>
    public ProductRole ProductRole { get; set; } = ProductRole.Full;

    /// <summary>
    /// Request credit score
    /// </summary>
    public bool RequestScore { get; set; } = false;
}
