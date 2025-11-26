using CRIF_API.Client.Models.Requests;
using CRIF_API.Client.Models.Responses;

namespace CRIF_API.Client.Services;

/// <summary>
/// CRIF Credit Bureau Service Interface
/// Main service for credit bureau operations
/// </summary>
public interface ICrifCreditBureauService
{
    /// <summary>
    /// New Application Enquiry - Submit new credit application and get credit report
    /// </summary>
    /// <param name="request">NAE request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Credit report and application codes</returns>
    Task<NAEResponse> NewApplicationEnquiryAsync(
        NAERequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// New Application Enquiry Presentation - Same as NAE but returns PDF
    /// </summary>
    /// <param name="request">NAEP request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Credit report and PDF document</returns>
    Task<PresentationResponse> NewApplicationEnquiryPresentationAsync(
        NAEPRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Monitoring Enquiry - Get credit report for existing customer
    /// </summary>
    /// <param name="request">ME request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Credit report</returns>
    Task<MEResponse> MonitoringEnquiryAsync(
        MERequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Monitoring Enquiry Presentation - Same as ME but returns PDF
    /// </summary>
    /// <param name="request">MEP request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Credit report and PDF document</returns>
    Task<PresentationResponse> MonitoringEnquiryPresentationAsync(
        MEPRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Application Update Enquiry - Update previously submitted application
    /// </summary>
    /// <param name="request">AUE request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Update confirmation with before/after snapshots</returns>
    Task<AUEResponse> ApplicationUpdateEnquiryAsync(
        AUERequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Portfolio Alerts Enquiry - Get portfolio alerts
    /// </summary>
    /// <param name="request">PAE request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of alerts</returns>
    Task<PAEResponse> PortfolioAlertsEnquiryAsync(
        PAERequest request,
        CancellationToken cancellationToken = default);
}
