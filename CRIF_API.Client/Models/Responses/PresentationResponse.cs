using CRIF_API.Client.Enums;

namespace CRIF_API.Client.Models.Responses;

/// <summary>
/// Presentation response (for NAEP and MEP)
/// </summary>
public class PresentationResponse : BaseResponse
{
    /// <summary>
    /// Same data as regular NAE/ME response
    /// </summary>
    public CreditReport? CreditReport { get; set; }

    /// <summary>
    /// Presentation culture (language)
    /// </summary>
    public PresentationCulture? PresentationCulture { get; set; }

    /// <summary>
    /// Presentation format type (PDF)
    /// </summary>
    public string? PresentationFormatType { get; set; }

    /// <summary>
    /// Base64-encoded PDF document
    /// </summary>
    public string? PresentationDocument { get; set; }
}
