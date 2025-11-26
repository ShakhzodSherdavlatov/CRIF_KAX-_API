namespace CRIF_API.Client.Services;

/// <summary>
/// SOAP client interface for CRIF Credit Bureau communication
/// </summary>
public interface ISoapClient
{
    /// <summary>
    /// Execute SOAP call
    /// </summary>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="operation">Operation name (NAE, ME, AUE, etc.)</param>
    /// <param name="request">Request object</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response object</returns>
    Task<TResponse> CallAsync<TRequest, TResponse>(
        string operation,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class;
}
