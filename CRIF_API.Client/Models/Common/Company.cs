namespace CRIF_API.Client.Models.Common;

/// <summary>
/// Company (legal entity) information
/// </summary>
public class Company
{
    /// <summary>
    /// Trade name (full legal name) - required
    /// </summary>
    public string TradeName { get; set; } = string.Empty;

    /// <summary>
    /// Short company name
    /// </summary>
    public string? ShortCompanyName { get; set; }

    /// <summary>
    /// Legal form (organizational-legal form) - see domain tables
    /// </summary>
    public string? LegalForm { get; set; }

    /// <summary>
    /// Registration place (required)
    /// </summary>
    public string RegistrationPlace { get; set; } = string.Empty;

    /// <summary>
    /// Economic activity code (required) - see domain tables
    /// </summary>
    public string EconomicActivity { get; set; } = string.Empty;

    /// <summary>
    /// Establishment date (required)
    /// </summary>
    public DateTime EstablishmentDate { get; set; }

    /// <summary>
    /// Gross income
    /// </summary>
    public decimal? GrossIncome { get; set; }

    /// <summary>
    /// Monthly expenses
    /// </summary>
    public decimal? MonthlyExpenses { get; set; }

    /// <summary>
    /// Net taxable income
    /// </summary>
    public decimal? NetTaxableIncome { get; set; }

    /// <summary>
    /// Number of employees - see domain tables
    /// </summary>
    public string? EmployeesNumber { get; set; }

    /// <summary>
    /// Addresses (at least one required)
    /// </summary>
    public List<Address> Addresses { get; set; } = new();

    /// <summary>
    /// Business identification codes (TIN, etc.)
    /// </summary>
    public List<IdentificationCode> IdentificationCodes { get; set; } = new();

    /// <summary>
    /// Contact information
    /// </summary>
    public List<Contact> Contacts { get; set; } = new();
}
