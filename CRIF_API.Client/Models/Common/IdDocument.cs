namespace CRIF_API.Client.Models.Common;

/// <summary>
/// ID document (passport, driver's license, etc.)
/// </summary>
public class IdDocument
{
    /// <summary>
    /// Document type code (see domain tables)
    /// Examples:
    /// - Passport of citizen of Uzbekistan
    /// - Foreign passport
    /// - Driver's license
    /// - ID card
    /// - Birth certificate
    /// </summary>
    public string IdType { get; set; } = string.Empty;

    /// <summary>
    /// Document number
    /// </summary>
    public string IdNumber { get; set; } = string.Empty;

    /// <summary>
    /// Issue date
    /// </summary>
    public DateTime? IssueDate { get; set; }

    /// <summary>
    /// Expiry date
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Issuing authority
    /// </summary>
    public string? IssuingAuthority { get; set; }
}
