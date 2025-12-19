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

    /// <summary>
    /// Test endpoint with pre-filled data from CRIF documentation example
    /// </summary>
    [HttpPost("test")]
    [ProducesResponseType(typeof(NewApplicationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TestApplication()
    {
        var request = new NewApplicationRequest
        {
            ProviderSubjectNo = "3333355555",
            SubjectRefDate = new DateTime(2014, 1, 1),
            Individual = new IndividualData
            {
                FirstName = "YUNUS",
                LastName = "KARIMOVA",
                MiddleName = "KARIMOVOVICH",
                DateOfBirth = new DateTime(1980, 1, 1),
                BirthPlace = "TASHKENT",
                Gender = "M",
                PINFL = "30212942400058",
                TIN = "541082877",
                Documents = new List<DocumentData>
                {
                    new DocumentData
                    {
                        DocumentType = "1",
                        DocumentNumber = "1234567890",
                        IssueDate = new DateTime(2000, 1, 1),
                        ExpirationDate = new DateTime(2020, 12, 31),
                        IssuingAuthority = "UZ"
                    },
                    new DocumentData
                    {
                        DocumentType = "2",
                        DocumentNumber = "AS0000000",
                        IssueDate = new DateTime(2001, 10, 10),
                        ExpirationDate = new DateTime(2020, 2, 2),
                        IssuingAuthority = "UZ"
                    },
                    new DocumentData
                    {
                        DocumentType = "3",
                        DocumentNumber = "PERRES000",
                        IssueDate = new DateTime(1999, 10, 10),
                        ExpirationDate = new DateTime(2024, 2, 2),
                        IssuingAuthority = "UZ"
                    }
                },
                Address = new AddressData
                {
                    City = "TASHKENT",
                    District = "226",
                    Region = "26",
                    Street = "29 NUKUS STREET",
                    Building = "",
                    Apartment = "",
                    Country = "UZ"
                },
                PhoneNumbers = new List<string>
                {
                    "+39 0335-11111111",
                    "0039051222222"
                },
                Employment = new Models.EmploymentData
                {
                    GrossIncome = 2000000,
                    Currency = "UZS",
                    OccupationStatus = "1",
                    DateHiredFrom = new DateTime(2001, 9, 3),
                    Occupation = "MANAGER",
                    EmployerName = "CRIF SPA"
                }
            },
            Application = new ApplicationInfo
            {
                ContractType = "53", // CreditCard
                ContractPhase = "Requested",
                ContractRequestDate = new DateTime(2013, 12, 11),
                Currency = "UZS",
                CreditLimit = 1000, // CreditCard uses CreditLimit, not FinancedAmount
                SubjectRole = "Borrower"
            },
            ProviderContractNo = "TEST-" + DateTime.Now.Ticks
        };

        return await CreateApplication(request);
    }

    #region Mapping Methods

    private NAERequest MapToNAERequest(NewApplicationRequest request)
    {
        var naeRequest = new NAERequest
        {
            ProviderSubjectNo = request.ProviderSubjectNo,
            SubjectRefDate = request.SubjectRefDate
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
                BirthPlace = request.Individual.BirthPlace,
                Gender = MapGender(request.Individual.Gender),
                IdentificationCodes = new List<IdentificationCode>(),
                Addresses = new List<Address>(),
                Contacts = new List<Contact>(),
                IdDocuments = new List<IdDocument>()
            };

            // Add PINFL if provided
            if (!string.IsNullOrWhiteSpace(request.Individual.PINFL))
            {
                naeRequest.Individual.IdentificationCodes.Add(new IdentificationCode
                {
                    IdentificationType = "1",
                    IdentificationNumber = request.Individual.PINFL
                });
            }

            // Add TIN if provided
            if (!string.IsNullOrWhiteSpace(request.Individual.TIN))
            {
                naeRequest.Individual.IdentificationCodes.Add(new IdentificationCode
                {
                    IdentificationType = "2",
                    IdentificationNumber = request.Individual.TIN
                });
            }

            // Add documents if provided
            if (request.Individual.Documents != null && request.Individual.Documents.Any())
            {
                foreach (var doc in request.Individual.Documents)
                {
                    naeRequest.Individual.IdDocuments.Add(new IdDocument
                    {
                        IdType = doc.DocumentType ?? "1",
                        IdNumber = doc.DocumentNumber ?? string.Empty,
                        IssueDate = doc.IssueDate,
                        ExpiryDate = doc.ExpirationDate,
                        IssuingAuthority = doc.IssuingAuthority
                    });
                }
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

            // Add phones if provided (each phone needs a different ContactType)
            if (request.Individual.PhoneNumbers != null && request.Individual.PhoneNumbers.Any())
            {
                int contactTypeIndex = 1;
                foreach (var phone in request.Individual.PhoneNumbers)
                {
                    if (!string.IsNullOrWhiteSpace(phone))
                    {
                        naeRequest.Individual.Contacts.Add(new Contact
                        {
                            ContactType = contactTypeIndex.ToString(),
                            ContactValue = phone
                        });
                        contactTypeIndex++;
                    }
                }
            }

            // Add employment if provided
            if (request.Individual.Employment != null)
            {
                naeRequest.Individual.Employment = new Client.Models.Common.EmploymentData
                {
                    GrossAnnualIncome = request.Individual.Employment.GrossIncome,
                    Currency = request.Individual.Employment.Currency,
                    OccupationStatus = request.Individual.Employment.OccupationStatus,
                    DateHiredFrom = request.Individual.Employment.DateHiredFrom,
                    Occupation = request.Individual.Employment.Occupation,
                    CompanyTradeName = request.Individual.Employment.EmployerName
                };
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
            ContractRequestDate = request.Application.ContractRequestDate,
            Currency = request.Application.Currency,
            FinancedAmount = request.Application.FinancedAmount,
            CreditLimit = request.Application.CreditLimit,
            MonthlyPaymentAmount = request.Application.MonthlyPaymentAmount,
            InstallmentsNumber = request.Application.InstallmentsNumber,
            PaymentPeriodicity = request.Application.PaymentPeriodicity
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
