using System.Xml.Linq;
using CRIF_API.Client.Models.Responses;
using CRIF_API.Client.Exceptions;

namespace CRIF_API.Client.Services;

/// <summary>
/// Parses SOAP XML responses from CRIF
/// Based on examples from Manual Section 9
/// </summary>
public class SoapXmlParser
{
    private static readonly XNamespace MgNs = "urn:cbs-messagegatewaysoap:2015-01-01";
    private static readonly XNamespace CbNs = "urn:crif-creditbureau:v1";

    public NAEResponse ParseNAEResponse(string xml)
    {
        var doc = XDocument.Parse(xml);
        var response = new NAEResponse();

        // Check for errors first
        CheckForErrors(doc, response);

        if (!response.Success)
            return response;

        var productOutput = doc.Descendants(CbNs + "CB_NAE_ProductOutput").FirstOrDefault();
        if (productOutput == null)
        {
            response.Success = false;
            response.Error = new ErrorInfo { ErrorDescription = "ProductOutput not found in response" };
            return response;
        }

        // Parse Application Codes
        var appCodes = productOutput.Descendants(CbNs + "ApplicationCodes").FirstOrDefault();
        if (appCodes != null)
        {
            response.ApplicationCodes = new ApplicationCodes
            {
                CBContractCode = appCodes.Attribute("CBContractCode")?.Value,
                ProviderContractNo = appCodes.Attribute("ProviderContractNo")?.Value,
                ProviderApplicationNo = appCodes.Attribute("ProviderApplicationNo")?.Value
            };
        }

        // Parse Credit Report
        var creditReport = productOutput.Descendants(CbNs + "CreditReport").FirstOrDefault();
        if (creditReport != null)
        {
            response.CreditReport = ParseCreditReport(creditReport);

            // Extract CBSubjectCode from matched subject
            if (response.CreditReport?.MatchedSubject?.CBSubjectCode != null && response.ApplicationCodes != null)
            {
                response.ApplicationCodes.CBSubjectCode = response.CreditReport.MatchedSubject.CBSubjectCode;
            }
        }

        response.Success = true;
        return response;
    }

    public MEResponse ParseMEResponse(string xml)
    {
        var doc = XDocument.Parse(xml);
        var response = new MEResponse();

        CheckForErrors(doc, response);

        if (!response.Success)
            return response;

        var productOutput = doc.Descendants(CbNs + "CB_ME_ProductOutput").FirstOrDefault();
        if (productOutput == null)
        {
            response.Success = false;
            response.Error = new ErrorInfo { ErrorDescription = "ProductOutput not found in response" };
            return response;
        }

        // Parse Credit Report
        var creditReport = productOutput.Descendants(CbNs + "CreditReport").FirstOrDefault();
        if (creditReport != null)
        {
            response.CreditReport = ParseCreditReport(creditReport);
        }

        response.Success = true;
        return response;
    }

    public AUEResponse ParseAUEResponse(string xml)
    {
        var doc = XDocument.Parse(xml);
        var response = new AUEResponse();

        CheckForErrors(doc, response);

        if (!response.Success)
            return response;

        var productOutput = doc.Descendants(CbNs + "CB_AUE_ProductOutput").FirstOrDefault();
        if (productOutput == null)
        {
            response.Success = false;
            response.Error = new ErrorInfo { ErrorDescription = "ProductOutput not found in response" };
            return response;
        }

        // Parse Application Codes
        var appCodes = productOutput.Descendants(CbNs + "ApplicationCodes").FirstOrDefault();
        if (appCodes != null)
        {
            response.ApplicationCodes = new ApplicationCodes
            {
                CBContractCode = appCodes.Attribute("CBContractCode")?.Value,
                ProviderContractNo = appCodes.Attribute("ProviderContractNo")?.Value
            };
        }

        response.Success = true;
        return response;
    }

    public PAEResponse ParsePAEResponse(string xml)
    {
        var doc = XDocument.Parse(xml);
        var response = new PAEResponse();

        CheckForErrors(doc, response);

        if (!response.Success)
            return response;

        var productOutput = doc.Descendants(CbNs + "CB_PAE_ProductOutput").FirstOrDefault();
        if (productOutput == null)
        {
            response.Success = false;
            response.Error = new ErrorInfo { ErrorDescription = "ProductOutput not found in response" };
            return response;
        }

        // Parse alerts
        var alerts = productOutput.Descendants(CbNs + "Alert");
        foreach (var alert in alerts)
        {
            response.Alerts.Add(new Alert
            {
                AlertCode = alert.Attribute("AlertCode")?.Value,
                AlertDescription = alert.Attribute("AlertDescription")?.Value,
                EventDateTime = DateTime.TryParse(alert.Attribute("EventDateTime")?.Value, out var dt) ? dt : DateTime.MinValue,
                CBSubjectCode = alert.Attribute("CBSubjectCode")?.Value,
                CBContractCode = alert.Attribute("CBContractCode")?.Value,
                ProviderContractNo = alert.Attribute("ProviderContractNo")?.Value,
                SubjectName = alert.Attribute("SubjectName")?.Value,
                Details = alert.Value
            });
        }

        response.TotalCount = response.Alerts.Count;
        response.Success = true;
        return response;
    }

    private CreditReport ParseCreditReport(XElement creditReportElement)
    {
        var report = new CreditReport();

        // Parse Matched Subject
        var matchedSubject = creditReportElement.Descendants(CbNs + "MatchedSubject").FirstOrDefault();
        if (matchedSubject != null)
        {
            report.MatchedSubject = new MatchedSubject
            {
                CBSubjectCode = matchedSubject.Attribute("CBSubjectCode")?.Value,
                FlagMatched = matchedSubject.Attribute("FlagMatched")?.Value == "1"
            };
        }

        // Parse Contract History
        var contractHistory = creditReportElement.Descendants(CbNs + "ContractHistory").FirstOrDefault();
        if (contractHistory != null)
        {
            report.ContractHistory = ParseContractHistory(contractHistory);
        }

        // Parse Footprint
        var footprint = creditReportElement.Descendants(CbNs + "Footprint").FirstOrDefault();
        if (footprint != null)
        {
            report.Footprint = ParseFootprint(footprint);
        }

        return report;
    }

    private ContractHistory ParseContractHistory(XElement contractHistoryElement)
    {
        var history = new ContractHistory();

        // Parse Aggregated Data
        var aggregated = contractHistoryElement.Descendants(CbNs + "AggregatedData").FirstOrDefault();
        if (aggregated != null)
        {
            history.AggregatedData = new AggregatedData
            {
                TotalContracts = int.TryParse(aggregated.Attribute("TotalContracts")?.Value, out var tc) ? tc : 0,
                TotalProviders = int.TryParse(aggregated.Attribute("TotalProviders")?.Value, out var tp) ? tp : 0,
                Currency = aggregated.Attribute("Currency")?.Value ?? "UZS"
            };
        }

        // Parse Not Granted Contracts
        var notGranted = contractHistoryElement.Descendants(CbNs + "NotGrantedContract");
        foreach (var contract in notGranted)
        {
            history.NotGrantedContracts.Add(new NotGrantedContract
            {
                CBContractCode = contract.Attribute("CBContractCode")?.Value,
                ProviderContractNo = contract.Attribute("ProviderContractNo")?.Value,
                ContractType = contract.Attribute("ContractType")?.Value,
                LastUpdateDate = DateTime.TryParse(contract.Attribute("LastUpdateDate")?.Value, out var date) ? date : null
            });
        }

        // Parse Granted Contracts
        var granted = contractHistoryElement.Descendants(CbNs + "GrantedContract");
        foreach (var contract in granted)
        {
            var grantedContract = new GrantedContract
            {
                CBContractCode = contract.Attribute("CBContractCode")?.Value,
                ProviderContractNo = contract.Attribute("ProviderContractNo")?.Value,
                ContractType = contract.Attribute("ContractType")?.Value,
                ProviderName = contract.Attribute("ProviderName")?.Value,
                LastUpdateDate = DateTime.TryParse(contract.Attribute("LastUpdateDate")?.Value, out var date) ? date : null
            };

            // Parse payment history
            var payments = contract.Descendants(CbNs + "PaymentHistory");
            foreach (var payment in payments)
            {
                grantedContract.PaymentHistory.Add(new PaymentHistory
                {
                    ReferenceYear = int.TryParse(payment.Attribute("ReferenceYear")?.Value, out var year) ? year : 0,
                    ReferenceMonth = int.TryParse(payment.Attribute("ReferenceMonth")?.Value, out var month) ? month : 0,
                    OutstandingBalance = decimal.TryParse(payment.Attribute("OutstandingBalance")?.Value, out var balance) ? balance : 0,
                    DaysPastDue = int.TryParse(payment.Attribute("DaysPastDue")?.Value, out var days) ? days : null,
                    Status = payment.Attribute("Status")?.Value
                });
            }

            history.GrantedContracts.Add(grantedContract);
        }

        return history;
    }

    private Footprint ParseFootprint(XElement footprintElement)
    {
        var footprint = new Footprint();

        // Parse counters
        var counters = footprintElement.Descendants(CbNs + "Counters").FirstOrDefault();
        if (counters != null)
        {
            footprint.Counters = new FootprintCounters
            {
                Count1Month = int.TryParse(counters.Attribute("Count1Month")?.Value, out var c1) ? c1 : 0,
                Count3Months = int.TryParse(counters.Attribute("Count3Months")?.Value, out var c3) ? c3 : 0,
                Count6Months = int.TryParse(counters.Attribute("Count6Months")?.Value, out var c6) ? c6 : 0,
                Count12Months = int.TryParse(counters.Attribute("Count12Months")?.Value, out var c12) ? c12 : 0
            };
        }

        // Parse footprint data
        var dataElements = footprintElement.Descendants(CbNs + "FootprintData");
        foreach (var data in dataElements)
        {
            footprint.Data.Add(new FootprintData
            {
                EnquiryType = Enum.TryParse<Enums.EnquiryType>(data.Attribute("EnquiryType")?.Value, out var type)
                    ? type
                    : Enums.EnquiryType.NAE,
                EnquiryDate = DateTime.TryParse(data.Attribute("EnquiryDate")?.Value, out var date) ? date : DateTime.MinValue,
                InstituteName = data.Attribute("InstituteName")?.Value
            });
        }

        return footprint;
    }

    private void CheckForErrors(XDocument doc, BaseResponse response)
    {
        // Check MessageResponse for errors
        var msgResponse = doc.Descendants(MgNs + "MessageResponse").FirstOrDefault();
        if (msgResponse != null)
        {
            var resultCode = msgResponse.Attribute("ResultCode")?.Value;
            response.MessageResponseResultCode = resultCode;

            if (resultCode != "S") // S = Success
            {
                response.Success = false;
                response.Error = new ErrorInfo
                {
                    ErrorCode = resultCode,
                    ErrorDescription = msgResponse.Attribute("ResultDescription")?.Value
                };
                return;
            }
        }

        // Check ProductResponse for errors
        var productResponse = doc.Descendants(MgNs + "ProductResponse").FirstOrDefault();
        if (productResponse != null)
        {
            var resultCode = productResponse.Attribute("ResultCode")?.Value;
            response.ProductResponseResultCode = resultCode;

            if (resultCode != "S")
            {
                response.Success = false;
                response.Error = new ErrorInfo
                {
                    ErrorCode = resultCode,
                    ErrorDescription = productResponse.Attribute("ResultDescription")?.Value
                };
                return;
            }
        }

        // Check for Error elements
        var errorElement = doc.Descendants(CbNs + "Error").FirstOrDefault();
        if (errorElement != null)
        {
            response.Success = false;
            response.Error = new ErrorInfo
            {
                ErrorCode = errorElement.Attribute("Code")?.Value,
                ErrorDescription = errorElement.Attribute("Description")?.Value,
                FieldName = errorElement.Attribute("FieldName")?.Value
            };
        }
    }
}
