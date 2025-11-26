namespace CRIF_API.Client.Models.Common;

/// <summary>
/// Contact information
/// </summary>
public class Contact
{
    /// <summary>
    /// Contact type (see domain tables)
    /// Examples: Mobile, Home phone, Work phone, Email
    /// </summary>
    public string ContactType { get; set; } = string.Empty;

    /// <summary>
    /// Contact value (phone number, email, etc.)
    /// </summary>
    public string ContactValue { get; set; } = string.Empty;
}
