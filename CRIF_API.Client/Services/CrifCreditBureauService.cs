using CRIF_API.Client.Configuration;
using CRIF_API.Client.Exceptions;
using CRIF_API.Client.Models.Requests;
using CRIF_API.Client.Models.Responses;
using Microsoft.Extensions.Options;

namespace CRIF_API.Client.Services;

/// <summary>
/// CRIF Credit Bureau Service Implementation
/// </summary>
public class CrifCreditBureauService : ICrifCreditBureauService
{
    private readonly CrifSettings _settings;
    private readonly ISoapClient _soapClient;

    public CrifCreditBureauService(
        IOptions<CrifSettings> settings,
        ISoapClient soapClient)
    {
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _soapClient = soapClient ?? throw new ArgumentNullException(nameof(soapClient));

        ValidateSettings();
    }

    public async Task<NAEResponse> NewApplicationEnquiryAsync(
        NAERequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateNAERequest(request);

        try
        {
            // TODO: Implement SOAP call
            // This is a skeleton - actual implementation will call SOAP service
            var response = await _soapClient.CallAsync<NAERequest, NAEResponse>(
                "NAE",
                request,
                cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            throw new CrifCommunicationException("Failed to execute NAE request", ex);
        }
    }

    public async Task<PresentationResponse> NewApplicationEnquiryPresentationAsync(
        NAEPRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateNAERequest(request);

        try
        {
            var response = await _soapClient.CallAsync<NAEPRequest, PresentationResponse>(
                "NAEP",
                request,
                cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            throw new CrifCommunicationException("Failed to execute NAEP request", ex);
        }
    }

    public async Task<MEResponse> MonitoringEnquiryAsync(
        MERequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateMERequest(request);

        try
        {
            var response = await _soapClient.CallAsync<MERequest, MEResponse>(
                "ME",
                request,
                cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            throw new CrifCommunicationException("Failed to execute ME request", ex);
        }
    }

    public async Task<PresentationResponse> MonitoringEnquiryPresentationAsync(
        MEPRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateMERequest(request);

        try
        {
            var response = await _soapClient.CallAsync<MEPRequest, PresentationResponse>(
                "MEP",
                request,
                cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            throw new CrifCommunicationException("Failed to execute MEP request", ex);
        }
    }

    public async Task<AUEResponse> ApplicationUpdateEnquiryAsync(
        AUERequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateAUERequest(request);

        try
        {
            var response = await _soapClient.CallAsync<AUERequest, AUEResponse>(
                "AUE",
                request,
                cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            throw new CrifCommunicationException("Failed to execute AUE request", ex);
        }
    }

    public async Task<PAEResponse> PortfolioAlertsEnquiryAsync(
        PAERequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _soapClient.CallAsync<PAERequest, PAEResponse>(
                "PAE",
                request,
                cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            throw new CrifCommunicationException("Failed to execute PAE request", ex);
        }
    }

    #region Validation Methods

    private void ValidateSettings()
    {
        if (string.IsNullOrWhiteSpace(_settings.EndpointUrl))
            throw new CrifConfigurationException("Endpoint URL is required");

        if (string.IsNullOrWhiteSpace(_settings.UserId))
            throw new CrifConfigurationException("User ID is required");

        if (string.IsNullOrWhiteSpace(_settings.Password))
            throw new CrifConfigurationException("Password is required");
    }

    private void ValidateNAERequest(NAERequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.Individual == null && request.Company == null)
            throw new CrifValidationException("Either Individual or Company data must be provided");

        if (request.Application != null)
        {
            if (string.IsNullOrWhiteSpace(request.Application.ContractType))
                throw new CrifValidationException("Contract type is required");
        }

        // Validate application codes
        if (!string.IsNullOrWhiteSpace(request.ApplicationCodes?.CBContractCode))
        {
            // Adding subject to existing contract
            if (!string.IsNullOrWhiteSpace(request.ApplicationCodes.ProviderContractNo) ||
                !string.IsNullOrWhiteSpace(request.ApplicationCodes.ProviderApplicationNo))
            {
                throw new CrifValidationException("When CBContractCode is provided, ProviderContractNo and ProviderApplicationNo must be empty");
            }
        }
    }

    private void ValidateMERequest(MERequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Must provide either codes or subject data
        if (string.IsNullOrWhiteSpace(request.CBSubjectCode) &&
            string.IsNullOrWhiteSpace(request.ProviderSubjectNo) &&
            request.Individual == null &&
            request.Company == null)
        {
            throw new CrifValidationException("Must provide either CB Subject Code, Provider Subject No, or subject data");
        }
    }

    private void ValidateAUERequest(AUERequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Must provide one application code
        if (string.IsNullOrWhiteSpace(request.ApplicationCodes?.CBContractCode) &&
            string.IsNullOrWhiteSpace(request.ApplicationCodes?.ProviderContractNo) &&
            string.IsNullOrWhiteSpace(request.ApplicationCodes?.ProviderApplicationNo))
        {
            throw new CrifValidationException("Must provide one application code (CBContractCode, ProviderContractNo, or ProviderApplicationNo)");
        }

        // Only one code should be provided
        int codeCount = 0;
        if (!string.IsNullOrWhiteSpace(request.ApplicationCodes?.CBContractCode)) codeCount++;
        if (!string.IsNullOrWhiteSpace(request.ApplicationCodes?.ProviderContractNo)) codeCount++;
        if (!string.IsNullOrWhiteSpace(request.ApplicationCodes?.ProviderApplicationNo)) codeCount++;

        if (codeCount > 1)
            throw new CrifValidationException("Only one application code should be provided");
    }

    #endregion
}
