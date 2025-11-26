namespace CRIF_API.Client.Exceptions;

/// <summary>
/// Base exception for CRIF API errors
/// </summary>
public class CrifException : Exception
{
    public string? ErrorCode { get; set; }
    public string? FieldName { get; set; }

    public CrifException(string message) : base(message)
    {
    }

    public CrifException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public CrifException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }

    public CrifException(string errorCode, string message, string fieldName) : base(message)
    {
        ErrorCode = errorCode;
        FieldName = fieldName;
    }
}

/// <summary>
/// Authentication/authorization error
/// </summary>
public class CrifAuthenticationException : CrifException
{
    public CrifAuthenticationException(string message) : base(message)
    {
    }
}

/// <summary>
/// SOAP communication error
/// </summary>
public class CrifCommunicationException : CrifException
{
    public CrifCommunicationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Validation error
/// </summary>
public class CrifValidationException : CrifException
{
    public CrifValidationException(string message) : base(message)
    {
    }

    public CrifValidationException(string errorCode, string message, string fieldName)
        : base(errorCode, message, fieldName)
    {
    }
}

/// <summary>
/// Configuration error
/// </summary>
public class CrifConfigurationException : CrifException
{
    public CrifConfigurationException(string message) : base(message)
    {
    }
}
