using CRIF_API.Client.Enums;

namespace CRIF_API.Client.Models.Common;

/// <summary>
/// Address information
/// </summary>
public class Address
{
    /// <summary>
    /// Address type (required)
    /// </summary>
    public AddressType AddressType { get; set; }

    /// <summary>
    /// Full address
    /// </summary>
    public string? FullAddress { get; set; }

    /// <summary>
    /// Street name and number (required for individuals)
    /// </summary>
    public string? StreetNameAndNumber { get; set; }

    /// <summary>
    /// Postal code
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// City (required)
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// District code (required) - see domain tables
    /// </summary>
    public string District { get; set; } = string.Empty;

    /// <summary>
    /// Region code (required) - see domain tables
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// Country code (required) - default: UZ
    /// </summary>
    public string Country { get; set; } = "UZ";

    /// <summary>
    /// Occupied since date
    /// </summary>
    public DateTime? OccupiedSince { get; set; }
}
