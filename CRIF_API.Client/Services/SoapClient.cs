using System.Text;
using CRIF_API.Client.Configuration;
using CRIF_API.Client.Exceptions;
using CRIF_API.Client.Models.Requests;
using CRIF_API.Client.Models.Responses;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace CRIF_API.Client.Services;

/// <summary>
/// SOAP client implementation for CRIF Credit Bureau
/// Handles HTTP communication and delegates XML building/parsing
/// </summary>
public class SoapClient : ISoapClient, IDisposable
{
    private readonly CrifSettings _settings;
    private readonly HttpClient _httpClient;
    private readonly SoapXmlBuilder _xmlBuilder;
    private readonly SoapXmlParser _xmlParser;
    private readonly ILogger<SoapClient> _logger;

    public SoapClient(IOptions<CrifSettings> settings, ILogger<SoapClient> logger)
    {
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds)
        };

        _xmlBuilder = new SoapXmlBuilder(_settings);
        _xmlParser = new SoapXmlParser();
    }

    public async Task<TResponse> CallAsync<TRequest, TResponse>(
        string operation,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class
    {
        try
        {
            // Build SOAP XML request
            string soapRequest = BuildRequest(operation, request);

            if (_settings.EnableLogging)
            {
                _logger.LogInformation("CRIF SOAP Request - Operation: {Operation}, Endpoint: {Endpoint}", operation, _settings.EndpointUrl);
                _logger.LogDebug("CRIF SOAP Request XML:\n{SoapXml}", soapRequest);
            }

            // Create HTTP request
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, _settings.EndpointUrl)
            {
                Content = new StringContent(soapRequest, Encoding.UTF8, "text/xml")
            };

            // Add SOAPAction header (may be required by some SOAP services)
            httpRequest.Headers.Add("SOAPAction", $"\"{operation}\"");

            // Execute HTTP call
            var httpResponse = await _httpClient.SendAsync(httpRequest, cancellationToken);

            // Read response
            var soapResponse = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (_settings.EnableLogging)
            {
                _logger.LogInformation("CRIF SOAP Response - Status: {StatusCode}", httpResponse.StatusCode);
                _logger.LogDebug("CRIF SOAP Response XML:\n{SoapXml}", soapResponse);
            }

            // Check HTTP status
            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new CrifCommunicationException(
                    $"HTTP error {httpResponse.StatusCode}: {soapResponse}",
                    new Exception($"Status code: {httpResponse.StatusCode}")
                );
            }

            // Parse response
            var response = ParseResponse<TRequest, TResponse>(operation, soapResponse);

            return response;
        }
        catch (HttpRequestException httpEx)
        {
            throw new CrifCommunicationException($"HTTP error: {httpEx.Message}", httpEx);
        }
        catch (TaskCanceledException timeoutEx)
        {
            throw new CrifCommunicationException("Request timeout", timeoutEx);
        }
        catch (CrifException)
        {
            // Re-throw CRIF exceptions as-is
            throw;
        }
        catch (Exception ex)
        {
            throw new CrifCommunicationException($"SOAP call failed: {ex.Message}", ex);
        }
    }

    private string BuildRequest<TRequest>(string operation, TRequest request)
        where TRequest : class
    {
        try
        {
            // NAEPRequest and MEPRequest inherit from NAE/ME, so check them first
            return request switch
            {
                NAEPRequest naepReq => _xmlBuilder.BuildNAERequest(naepReq), // Check NAEP first
                NAERequest naeReq => _xmlBuilder.BuildNAERequest(naeReq),
                MEPRequest mepReq => _xmlBuilder.BuildMERequest(mepReq), // Check MEP first
                MERequest meReq => _xmlBuilder.BuildMERequest(meReq),
                AUERequest aueReq => _xmlBuilder.BuildAUERequest(aueReq),
                _ => throw new NotSupportedException($"Request type {typeof(TRequest).Name} is not supported")
            };
        }
        catch (Exception ex)
        {
            throw new CrifException($"Failed to build SOAP request: {ex.Message}", ex);
        }
    }

    private TResponse ParseResponse<TRequest, TResponse>(string operation, string soapResponse)
        where TRequest : class
        where TResponse : class
    {
        try
        {
            object response = typeof(TResponse).Name switch
            {
                nameof(NAEResponse) => _xmlParser.ParseNAEResponse(soapResponse),
                nameof(MEResponse) => _xmlParser.ParseMEResponse(soapResponse),
                nameof(AUEResponse) => _xmlParser.ParseAUEResponse(soapResponse),
                nameof(PAEResponse) => _xmlParser.ParsePAEResponse(soapResponse),
                nameof(PresentationResponse) => ParsePresentationResponse(soapResponse, typeof(TRequest)),
                _ => throw new NotSupportedException($"Response type {typeof(TResponse).Name} is not supported")
            };

            return (TResponse)response;
        }
        catch (Exception ex)
        {
            throw new CrifException($"Failed to parse SOAP response: {ex.Message}", ex);
        }
    }

    private PresentationResponse ParsePresentationResponse(string soapResponse, Type requestType)
    {
        // For presentation (PDF) responses, parse like regular response
        // but also extract the PDF document
        bool isNAE = requestType == typeof(NAEPRequest) || requestType == typeof(NAERequest);

        var presentationResponse = new PresentationResponse();

        if (isNAE)
        {
            var naeResponse = _xmlParser.ParseNAEResponse(soapResponse);
            presentationResponse.Success = naeResponse.Success;
            presentationResponse.Error = naeResponse.Error;
            presentationResponse.CreditReport = naeResponse.CreditReport;
            presentationResponse.MessageResponseResultCode = naeResponse.MessageResponseResultCode;
            presentationResponse.ProductResponseResultCode = naeResponse.ProductResponseResultCode;
        }
        else
        {
            var meResponse = _xmlParser.ParseMEResponse(soapResponse);
            presentationResponse.Success = meResponse.Success;
            presentationResponse.Error = meResponse.Error;
            presentationResponse.CreditReport = meResponse.CreditReport;
            presentationResponse.MessageResponseResultCode = meResponse.MessageResponseResultCode;
            presentationResponse.ProductResponseResultCode = meResponse.ProductResponseResultCode;
        }

        // TODO: Extract PDF document from response
        // Look for <PresentationDocument> element with Base64-encoded PDF
        // presentationResponse.PresentationDocument = extractedBase64Pdf;

        return presentationResponse;
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
