using Microsoft.AspNetCore.Mvc;
using CRIF_API.Client.Services;
using CRIF_API.Client.Models.Requests;
using CRIF_API.Client.Models.Common;
using CRIF_API.Client.Enums;
using CRIF_API.Client.Exceptions;
using CRIF_API.RestApi.Models;

namespace CRIF_API.RestApi.Controllers;

/// <summary>
/// Credit Bureau API Controller
/// Provides REST endpoints for CRIF Credit Bureau integration
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CreditBureauController : ControllerBase
{
    private readonly ICrifCreditBureauService _crifService;
    private readonly ILogger<CreditBureauController> _logger;

    public CreditBureauController(
        ICrifCreditBureauService crifService,
        ILogger<CreditBureauController> logger)
    {
        _crifService = crifService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new credit application enquiry
    /// </summary>
    /// <param name="request">Application details</param>
    /// <returns>Credit bureau response with codes and report</returns>
    [HttpPost("applications")]
    [ProducesResponseType(typeof(NewApplicationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateApplication([FromBody] NewApplicationRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new credit application");

            // Validate request
            if (request.Individual == null && request.Company == null)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Either Individual or Company data must be provided"
                });
            }

            // Map REST API request to CRIF SOAP request
            var naeRequest = MapToNAERequest(request);

            // Call CRIF service
            var naeResponse = await _crifService.NewApplicationEnquiryAsync(naeRequest);

            // Map CRIF response to REST API response
            var response = MapToRestResponse(naeResponse);

            _logger.LogInformation("Application created successfully. CB Contract Code: {Code}",
                response.CBContractCode);

            return Ok(response);
        }
        catch (CrifValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error");
            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "VALIDATION_ERROR"
            });
        }
        catch (CrifAuthenticationException ex)
        {
            _logger.LogError(ex, "Authentication error");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Authentication failed with credit bureau",
                ErrorCode = "AUTH_ERROR"
            });
        }
        catch (CrifCommunicationException ex)
        {
            _logger.LogError(ex, "Communication error");
            return StatusCode(500, new ErrorResponse
            {
                Message = "Failed to communicate with credit bureau",
                ErrorCode = "COMMUNICATION_ERROR"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating application");
            return StatusCode(500, new ErrorResponse
            {
                Message = "An unexpected error occurred",
                ErrorCode = "INTERNAL_ERROR"
            });
        }
    }

    /// <summary>
    /// Get service health status
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetHealth()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    #region Mapping Methods

    private NAERequest MapToNAERequest(NewApplicationRequest request)
    {
        var naeRequest = new NAERequest
        {
            ProviderSubjectNo = request.ProviderSubjectNo
        };

        // Map Individual data
        if (request.Individual != null)
        {
            naeRequest.Individual = new Individual
            {
                FirstName = request.Individual.FirstName,
                LastName = request.Individual.LastName,
                Patronymic = request.Individual.MiddleName,
                DateOfBirth = request.Individual.DateOfBirth,
                Gender = MapGender(request.Individual.Gender),
                IdentificationCodes = new List<IdentificationCode>(),
                Addresses = new List<Address>(),
                Contacts = new List<Contact>()
            };

            // Add PINFL if provided
            if (!string.IsNullOrWhiteSpace(request.Individual.PINFL))
            {
                naeRequest.Individual.IdentificationCodes.Add(new IdentificationCode
                {
                    IdentificationType = "PINFL",
                    IdentificationNumber = request.Individual.PINFL
                });
            }

            // Add document if provided
            if (request.Individual.Document != null)
            {
                naeRequest.Individual.IdDocuments = new List<IdDocument>
                {
                    new IdDocument
                    {
                        IdType = request.Individual.Document.DocumentType ?? "PASSPORT",
                        IdNumber = request.Individual.Document.DocumentNumber ?? string.Empty,
                        IssueDate = request.Individual.Document.IssueDate,
                        ExpiryDate = request.Individual.Document.ExpirationDate,
                        IssuingAuthority = request.Individual.Document.IssuingAuthority
                    }
                };
            }

            // Add address if provided
            if (request.Individual.Address != null)
            {
                naeRequest.Individual.Addresses.Add(new Address
                {
                    AddressType = AddressType.Residential,
                    City = request.Individual.Address.City ?? string.Empty,
                    District = request.Individual.Address.District ?? string.Empty,
                    Region = request.Individual.Address.Region ?? string.Empty,
                    StreetNameAndNumber = $"{request.Individual.Address.Street} {request.Individual.Address.Building} {request.Individual.Address.Apartment}".Trim(),
                    Country = request.Individual.Address.Country
                });
            }

            // Add phone if provided
            if (!string.IsNullOrWhiteSpace(request.Individual.PhoneNumber))
            {
                naeRequest.Individual.Contacts.Add(new Contact
                {
                    ContactType = "Phone",
                    ContactValue = request.Individual.PhoneNumber
                });
            }
        }

        // Map Company data
        if (request.Company != null)
        {
            naeRequest.Company = new Company
            {
                TradeName = request.Company.TradeName,
                LegalForm = request.Company.LegalForm,
                RegistrationPlace = request.Company.RegistrationPlace,
                EconomicActivity = request.Company.EconomicActivity,
                EstablishmentDate = request.Company.EstablishmentDate ?? DateTime.Now.Date,
                IdentificationCodes = new List<IdentificationCode>(),
                Addresses = new List<Address>()
            };

            // Add TIN if provided
            if (!string.IsNullOrWhiteSpace(request.Company.TIN))
            {
                naeRequest.Company.IdentificationCodes.Add(new IdentificationCode
                {
                    IdentificationType = "TIN",
                    IdentificationNumber = request.Company.TIN
                });
            }

            // Add address if provided
            if (request.Company.Address != null)
            {
                naeRequest.Company.Addresses.Add(new Address
                {
                    AddressType = AddressType.Other,
                    City = request.Company.Address.City ?? string.Empty,
                    District = request.Company.Address.District ?? string.Empty,
                    Region = request.Company.Address.Region ?? string.Empty,
                    StreetNameAndNumber = $"{request.Company.Address.Street} {request.Company.Address.Building}".Trim(),
                    Country = request.Company.Address.Country
                });
            }
        }

        // Map Application data
        naeRequest.Application = new ApplicationData
        {
            ContractType = request.Application.ContractType,
            ContractPhase = MapContractPhase(request.Application.ContractPhase),
            Currency = request.Application.Currency,
            FinancedAmount = request.Application.FinancedAmount,
            MonthlyPaymentAmount = request.Application.MonthlyPaymentAmount
        };

        // Map Link data
        naeRequest.Link = new LinkData
        {
            RoleOfSubject = MapSubjectRole(request.Application.SubjectRole)
        };

        // Map Application codes
        if (!string.IsNullOrWhiteSpace(request.ProviderContractNo))
        {
            naeRequest.ApplicationCodes = new ApplicationCodes
            {
                ProviderContractNo = request.ProviderContractNo
            };
        }

        return naeRequest;
    }

    private NewApplicationResponse MapToRestResponse(Client.Models.Responses.NAEResponse naeResponse)
    {
        var response = new NewApplicationResponse
        {
            Success = naeResponse.Success,
            ErrorMessage = naeResponse.Error?.ErrorDescription,
            ErrorCode = naeResponse.Error?.ErrorCode,
            CBContractCode = naeResponse.ApplicationCodes?.CBContractCode,
            CBSubjectCode = naeResponse.ApplicationCodes?.CBSubjectCode,
            SubjectMatched = naeResponse.CreditReport?.MatchedSubject?.FlagMatched
        };

        // Map credit report summary if available
        if (naeResponse.CreditReport?.ContractHistory?.AggregatedData != null)
        {
            var aggregated = naeResponse.CreditReport.ContractHistory.AggregatedData;
            response.CreditReport = new CreditReportSummary
            {
                TotalActiveContracts = aggregated.TotalContracts,
                TotalDebtAmount = aggregated.AmountsSummary?.TotalDebitBalance,
                OverdueContracts = 0 // Not directly available in AggregatedData
            };
        }

        // Add score if available
        if (naeResponse.Score != null)
        {
            response.CreditReport ??= new CreditReportSummary();
            response.CreditReport.CreditScore = (int?)naeResponse.Score.ScoreValue;
            response.CreditReport.RiskClass = naeResponse.Score.ScoreDescription;
        }

        return response;
    }

    private ContractPhase MapContractPhase(string phase)
    {
        return phase?.ToUpper() switch
        {
            "REQUESTED" => ContractPhase.Requested,
            "ACTIVE" => ContractPhase.Active,
            "CLOSED" => ContractPhase.Closed,
            _ => ContractPhase.Requested
        };
    }

    private SubjectRole MapSubjectRole(string role)
    {
        return role?.ToUpper() switch
        {
            "BORROWER" => SubjectRole.Borrower,
            "COBORROWER" => SubjectRole.CoBorrower,
            "GUARANTOR" => SubjectRole.Guarantor,
            _ => SubjectRole.Borrower
        };
    }

    private Gender? MapGender(string? gender)
    {
        if (string.IsNullOrWhiteSpace(gender))
            return null;

        return gender.ToUpper() switch
        {
            "M" or "MALE" => Gender.Male,
            "F" or "FEMALE" => Gender.Female,
            _ => Gender.Unknown
        };
    }

    #endregion
}
