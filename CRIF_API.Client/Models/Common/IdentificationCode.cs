namespace CRIF_API.Client.Models.Common;

/// <summary>
/// Identification code (PINFL, TIN, etc.)
/// </summary>
public class IdentificationCode
{
    /// <summary>
    /// Identification type code (see domain tables)
    /// Examples: PINFL, TIN, etc.
    /// </summary>
    public string IdentificationType { get; set; } = string.Empty;

    /// <summary>
    /// Identification number
    /// </summary>
    public string IdentificationNumber { get; set; } = string.Empty;
}
