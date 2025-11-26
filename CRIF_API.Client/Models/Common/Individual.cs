using CRIF_API.Client.Enums;

namespace CRIF_API.Client.Models.Common;

/// <summary>
/// Individual (physical person) information
/// </summary>
public class Individual
{
    /// <summary>
    /// First name (required) - from official document
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name (required) - from official document
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Patronymic (middle name)
    /// </summary>
    public string? Patronymic { get; set; }

    /// <summary>
    /// Original last name (maiden name)
    /// </summary>
    public string? OriginalLastName { get; set; }

    /// <summary>
    /// Date of birth (required) - format: YYYY-MM-DD
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Birth place
    /// </summary>
    public string? BirthPlace { get; set; }

    /// <summary>
    /// Gender
    /// </summary>
    public Gender? Gender { get; set; }

    /// <summary>
    /// Marriage status
    /// </summary>
    public MarriageStatus? MarriageStatus { get; set; }

    /// <summary>
    /// Addresses (at least one required)
    /// </summary>
    public List<Address> Addresses { get; set; } = new();

    /// <summary>
    /// Identification codes (PINFL, etc.)
    /// </summary>
    public List<IdentificationCode> IdentificationCodes { get; set; } = new();

    /// <summary>
    /// ID documents (passport, etc.)
    /// </summary>
    public List<IdDocument> IdDocuments { get; set; } = new();

    /// <summary>
    /// Contact information
    /// </summary>
    public List<Contact> Contacts { get; set; } = new();

    /// <summary>
    /// Employment data
    /// </summary>
    public EmploymentData? Employment { get; set; }
}
