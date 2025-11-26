using CRIF_API.Client.Enums;

namespace CRIF_API.Client.Models.Requests;

/// <summary>
/// Monitoring Enquiry Presentation (MEP) Request
/// Same as ME but returns PDF format
/// </summary>
public class MEPRequest : MERequest
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
