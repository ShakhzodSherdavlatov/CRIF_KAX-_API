using CRIF_API.Client.Enums;

namespace CRIF_API.Client.Models.Requests;

/// <summary>
/// New Application Enquiry Presentation (NAEP) Request
/// Same as NAE but returns PDF format
/// </summary>
public class NAEPRequest : NAERequest
{
    /// <summary>
    /// Presentation culture (language)
    /// </summary>
    public PresentationCulture PresentationCulture { get; set; } = PresentationCulture.Russian;

    /// <summary>
    /// Presentation format type (always PDF)
    /// </summary>
    public string PresentationFormatType { get; set; } = "PDF";
}
