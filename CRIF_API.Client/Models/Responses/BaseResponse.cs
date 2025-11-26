namespace CRIF_API.Client.Models.Responses;

/// <summary>
/// Base response class for all CRIF API responses
/// </summary>
public abstract class BaseResponse
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error information (if any)
    /// </summary>
    public ErrorInfo? Error { get; set; }

    /// <summary>
    /// Message response result code
    /// </summary>
    public string? MessageResponseResultCode { get; set; }

    /// <summary>
    /// Credentials response result code
    /// </summary>
    public string? CredentialsResponseResultCode { get; set; }

    /// <summary>
    /// Product response result code
    /// </summary>
    public string? ProductResponseResultCode { get; set; }
}

/// <summary>
/// Error information
/// </summary>
public class ErrorInfo
{
    /// <summary>
    /// Error code
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Error description
    /// </summary>
    public string? ErrorDescription { get; set; }

    /// <summary>
    /// Field name (if field-specific error)
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// Additional error details
    /// </summary>
    public string? Details { get; set; }
}
