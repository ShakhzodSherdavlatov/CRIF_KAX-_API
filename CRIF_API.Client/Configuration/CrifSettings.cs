namespace CRIF_API.Client.Configuration;

/// <summary>
/// CRIF Credit Bureau configuration settings
/// </summary>
public class CrifSettings
{
    /// <summary>
    /// SOAP endpoint URL (UAT or PROD)
    /// </summary>
    public string EndpointUrl { get; set; } = string.Empty;

    /// <summary>
    /// User ID for authentication
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Password for authentication
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Request timeout in seconds (default 120)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 120;

    /// <summary>
    /// Environment (UAT or PROD)
    /// </summary>
    public string Environment { get; set; } = "UAT";

    /// <summary>
    /// Enable detailed logging
    /// </summary>
    public bool EnableLogging { get; set; } = true;
}
