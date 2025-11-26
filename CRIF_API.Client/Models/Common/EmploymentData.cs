namespace CRIF_API.Client.Models.Common;

/// <summary>
/// Employment information for individuals
/// </summary>
public class EmploymentData
{
    /// <summary>
    /// Occupation status (employment type) - see domain tables
    /// </summary>
    public string? OccupationStatus { get; set; }

    /// <summary>
    /// Company trade name (employer name)
    /// </summary>
    public string? CompanyTradeName { get; set; }

    /// <summary>
    /// Occupation (job title/position)
    /// </summary>
    public string? Occupation { get; set; }

    /// <summary>
    /// Gross annual income
    /// </summary>
    public decimal? GrossAnnualIncome { get; set; }

    /// <summary>
    /// Currency code (default: UZS)
    /// </summary>
    public string Currency { get; set; } = "UZS";

    /// <summary>
    /// Employment start date
    /// </summary>
    public DateTime? DateHiredFrom { get; set; }

    /// <summary>
    /// Employment end date
    /// </summary>
    public DateTime? DateHiredTo { get; set; }
}
