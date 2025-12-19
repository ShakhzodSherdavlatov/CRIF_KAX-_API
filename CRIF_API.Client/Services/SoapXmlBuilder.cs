using System.Text;
using System.Xml;
using System.Xml.Linq;
using CRIF_API.Client.Configuration;
using CRIF_API.Client.Constants;
using CRIF_API.Client.Models.Common;
using CRIF_API.Client.Models.Requests;

namespace CRIF_API.Client.Services;

/// <summary>
/// Builds SOAP XML requests according to CRIF specifications
/// Based on examples from Manual Section 9
/// </summary>
public class SoapXmlBuilder
{
    private readonly CrifSettings _settings;
    private static readonly XNamespace SoapNs = "http://schemas.xmlsoap.org/soap/envelope/";
    private static readonly XNamespace MgNs = "urn:cbs-messagegatewaysoap:2015-01-01";
    private static readonly XNamespace CbNs = "urn:crif-creditbureau:v1";

    public SoapXmlBuilder(CrifSettings settings)
    {
        _settings = settings;
    }

    public string BuildNAERequest(NAERequest request)
    {
        var doc = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(SoapNs + "Envelope",
                new XAttribute(XNamespace.Xmlns + "soapenv", SoapNs),
                new XAttribute(XNamespace.Xmlns + "urn", MgNs),
                new XAttribute(XNamespace.Xmlns + "cb", CbNs),
                new XElement(SoapNs + "Body",
                    new XElement(MgNs + "MGRequest",
                        BuildMessageElement(),
                        BuildProductElement("CB_NAE_Product", "",
                            BuildNAEProductInput(request))
                    )
                )
            )
        );

        return doc.ToString(SaveOptions.DisableFormatting);
    }

    public string BuildMERequest(MERequest request)
    {
        var doc = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(SoapNs + "Envelope",
                new XAttribute(XNamespace.Xmlns + "soapenv", SoapNs),
                new XAttribute(XNamespace.Xmlns + "urn", MgNs),
                new XAttribute(XNamespace.Xmlns + "cb", CbNs),
                new XElement(SoapNs + "Body",
                    new XElement(MgNs + "MGRequest",
                        BuildMessageElement(),
                        BuildProductElement("CB_ME_Product", "",
                            BuildMEProductInput(request))
                    )
                )
            )
        );

        return doc.ToString(SaveOptions.DisableFormatting);
    }

    public string BuildAUERequest(AUERequest request)
    {
        var doc = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(SoapNs + "Envelope",
                new XAttribute(XNamespace.Xmlns + "soapenv", SoapNs),
                new XAttribute(XNamespace.Xmlns + "urn", MgNs),
                new XAttribute(XNamespace.Xmlns + "cb", CbNs),
                new XElement(SoapNs + "Body",
                    new XElement(MgNs + "MGRequest",
                        BuildMessageElement(),
                        BuildProductElement("CB_AUE_Product", "",
                            BuildAUEProductInput(request))
                    )
                )
            )
        );

        return doc.ToString(SaveOptions.DisableFormatting);
    }

    private XElement BuildMessageElement()
    {
        return new XElement(MgNs + "Message",
            new XAttribute("GroupId", Guid.NewGuid().ToString()),
            new XAttribute("Id", Guid.NewGuid().ToString()),
            new XAttribute("TimeStamp", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
            new XAttribute("Idempotence", "unique"),
            new XElement(MgNs + "Credential",
                new XAttribute("Domain", ""),
                new XAttribute("Id", _settings.UserId),
                new XAttribute("Password", _settings.Password)
            )
        );
    }

    private XElement BuildProductElement(string productId, string version, XElement productInput)
    {
        return new XElement(MgNs + "Product",
            new XAttribute("ServiceId", "CB"),
            new XAttribute("Id", productId),
            new XAttribute("Version", version),
            productInput
        );
    }

    private XElement BuildNAEProductInput(NAERequest request)
    {
        var elements = new List<XElement>();

        // Subject
        var subjectAttrs = new List<XAttribute>();
        if (!string.IsNullOrWhiteSpace(request.ProviderSubjectNo))
            subjectAttrs.Add(new XAttribute("ProviderSubjectNo", request.ProviderSubjectNo));

        var subjectContent = new List<XElement>();

        if (request.Individual != null)
            subjectContent.Add(BuildIndividualElement(request.Individual));

        if (request.Company != null)
            subjectContent.Add(BuildCompanyElement(request.Company));

        elements.Add(new XElement(CbNs + "Subject", subjectAttrs, subjectContent));

        // Application
        if (request.Application != null)
            elements.Add(BuildApplicationElement(request.Application));

        // Link
        elements.Add(new XElement(CbNs + "Link",
            new XAttribute("Role", MapSubjectRole(request.Link.RoleOfSubject))
        ));

        // Application Codes
        var appCodeAttrs = new List<XAttribute>();
        if (!string.IsNullOrWhiteSpace(request.ApplicationCodes?.ProviderContractNo))
            appCodeAttrs.Add(new XAttribute("ProviderContractNo", request.ApplicationCodes.ProviderContractNo));
        if (!string.IsNullOrWhiteSpace(request.ApplicationCodes?.ProviderApplicationNo))
            appCodeAttrs.Add(new XAttribute("ProviderApplicationNo", request.ApplicationCodes.ProviderApplicationNo));
        if (!string.IsNullOrWhiteSpace(request.ApplicationCodes?.CBContractCode))
            appCodeAttrs.Add(new XAttribute("CBContractCode", request.ApplicationCodes.CBContractCode));

        if (appCodeAttrs.Any())
            elements.Add(new XElement(CbNs + "ApplicationCodes", appCodeAttrs));

        return new XElement(CbNs + "CB_NAE_ProductInput", elements);
    }

    private XElement BuildMEProductInput(MERequest request)
    {
        var elements = new List<XElement>();

        // Subject
        var subjectAttrs = new List<XAttribute>();
        if (!string.IsNullOrWhiteSpace(request.CBSubjectCode))
            subjectAttrs.Add(new XAttribute("CBSubjectCode", request.CBSubjectCode));
        if (!string.IsNullOrWhiteSpace(request.ProviderSubjectNo))
            subjectAttrs.Add(new XAttribute("ProviderSubjectNo", request.ProviderSubjectNo));

        var subjectContent = new List<XElement>();
        if (request.Individual != null)
            subjectContent.Add(BuildIndividualElement(request.Individual));
        if (request.Company != null)
            subjectContent.Add(BuildCompanyElement(request.Company));

        elements.Add(new XElement(CbNs + "Subject", subjectAttrs, subjectContent));

        return new XElement(CbNs + "CB_ME_ProductInput", elements);
    }

    private XElement BuildAUEProductInput(AUERequest request)
    {
        var elements = new List<XElement>();

        // Application Codes
        var appCodeAttrs = new List<XAttribute>();
        if (!string.IsNullOrWhiteSpace(request.ApplicationCodes?.CBContractCode))
            appCodeAttrs.Add(new XAttribute("CBContractCode", request.ApplicationCodes.CBContractCode));
        if (!string.IsNullOrWhiteSpace(request.ApplicationCodes?.ProviderContractNo))
            appCodeAttrs.Add(new XAttribute("ProviderContractNo", request.ApplicationCodes.ProviderContractNo));
        if (!string.IsNullOrWhiteSpace(request.ApplicationCodes?.ProviderApplicationNo))
            appCodeAttrs.Add(new XAttribute("ProviderApplicationNo", request.ApplicationCodes.ProviderApplicationNo));

        elements.Add(new XElement(CbNs + "ApplicationCodes", appCodeAttrs));

        // Application to update
        var appAttrs = new List<XAttribute>();
        if (request.ApplicationToUpdate.ContractPhase.HasValue)
            appCodeAttrs.Add(new XAttribute("ContractPhase", MapContractPhase(request.ApplicationToUpdate.ContractPhase.Value)));
        if (request.ApplicationToUpdate.CancellationFlag.HasValue)
            appAttrs.Add(new XAttribute("CancellationFlag", request.ApplicationToUpdate.CancellationFlag.Value ? "1" : "0"));
        if (request.ApplicationToUpdate.FinancedAmount.HasValue)
            appAttrs.Add(new XAttribute("FinancedAmount", request.ApplicationToUpdate.FinancedAmount.Value));
        if (request.ApplicationToUpdate.CreditLimit.HasValue)
            appAttrs.Add(new XAttribute("CreditLimit", request.ApplicationToUpdate.CreditLimit.Value));

        if (appAttrs.Any())
            elements.Add(new XElement(CbNs + "Application", appAttrs));

        return new XElement(CbNs + "CB_AUE_ProductInput", elements);
    }

    private XElement BuildIndividualElement(Individual individual)
    {
        var attrs = new List<XAttribute>();
        if (individual.Gender.HasValue)
            attrs.Add(new XAttribute("Gender", MapGender(individual.Gender.Value)));

        var elements = new List<XElement>();

        // Name
        elements.Add(new XElement(CbNs + "IndividualName",
            new XAttribute("FirstName", individual.FirstName),
            new XAttribute("LastName", individual.LastName),
            !string.IsNullOrWhiteSpace(individual.Patronymic)
                ? new XAttribute("Patronymic", individual.Patronymic)
                : null
        ));

        // Birth data
        elements.Add(new XElement(CbNs + "BirthData",
            new XAttribute("Date", individual.DateOfBirth.ToString("yyyy-MM-dd")),
            !string.IsNullOrWhiteSpace(individual.BirthPlace)
                ? new XAttribute("Place", individual.BirthPlace)
                : null,
            new XAttribute("Country", "UZ")
        ));

        // Addresses
        foreach (var addr in individual.Addresses)
        {
            elements.Add(BuildAddressElement(addr));
        }

        // Identification codes
        foreach (var idCode in individual.IdentificationCodes)
        {
            elements.Add(new XElement(CbNs + "IdentificationCode",
                new XAttribute("Type", idCode.IdentificationType),
                new XAttribute("Number", idCode.IdentificationNumber)
            ));
        }

        // ID documents
        foreach (var idDoc in individual.IdDocuments)
        {
            var idAttrs = new List<XAttribute>
            {
                new XAttribute("Type", idDoc.IdType),
                new XAttribute("Number", idDoc.IdNumber)
            };
            if (idDoc.IssueDate.HasValue)
                idAttrs.Add(new XAttribute("IssueDate", idDoc.IssueDate.Value.ToString("yyyy-MM-dd")));
            if (idDoc.ExpiryDate.HasValue)
                idAttrs.Add(new XAttribute("ExpiryDate", idDoc.ExpiryDate.Value.ToString("yyyy-MM-dd")));
            idAttrs.Add(new XAttribute("IssueCountry", "UZ"));

            elements.Add(new XElement(CbNs + "ID", idAttrs));
        }

        // Contacts
        foreach (var contact in individual.Contacts)
        {
            elements.Add(new XElement(CbNs + "Contact",
                new XAttribute("Type", contact.ContactType),
                new XAttribute("Value", contact.ContactValue)
            ));
        }

        // Employment
        if (individual.Employment != null)
        {
            var empAttrs = new List<XAttribute>();
            if (individual.Employment.GrossAnnualIncome.HasValue)
                empAttrs.Add(new XAttribute("GrossIncome", individual.Employment.GrossAnnualIncome.Value));
            empAttrs.Add(new XAttribute("Currency", individual.Employment.Currency));
            if (!string.IsNullOrWhiteSpace(individual.Employment.OccupationStatus))
                empAttrs.Add(new XAttribute("OccupationStatus", individual.Employment.OccupationStatus));
            if (individual.Employment.DateHiredFrom.HasValue)
                empAttrs.Add(new XAttribute("DateHiredFrom", individual.Employment.DateHiredFrom.Value.ToString("yyyy-MM-dd")));
            if (!string.IsNullOrWhiteSpace(individual.Employment.Occupation))
                empAttrs.Add(new XAttribute("Occupation", individual.Employment.Occupation));

            var empChildren = new List<XElement>();
            if (!string.IsNullOrWhiteSpace(individual.Employment.CompanyTradeName))
            {
                empChildren.Add(new XElement(CbNs + "EmployerData",
                    new XElement(CbNs + "CompanyName",
                        new XAttribute("TradeName", individual.Employment.CompanyTradeName)
                    )
                ));
            }

            elements.Add(new XElement(CbNs + "EmploymentData", empAttrs, empChildren));
        }

        return new XElement(CbNs + "Individual", attrs, elements);
    }

    private XElement BuildCompanyElement(Company company)
    {
        var elements = new List<XElement>();

        // Company name
        elements.Add(new XElement(CbNs + "CompanyName",
            new XAttribute("TradeName", company.TradeName)
        ));

        // Addresses
        foreach (var addr in company.Addresses)
        {
            elements.Add(BuildAddressElement(addr));
        }

        // Identification codes
        foreach (var idCode in company.IdentificationCodes)
        {
            elements.Add(new XElement(CbNs + "IdentificationCode",
                new XAttribute("Type", idCode.IdentificationType),
                new XAttribute("Number", idCode.IdentificationNumber)
            ));
        }

        // Contacts
        foreach (var contact in company.Contacts)
        {
            elements.Add(new XElement(CbNs + "Contact",
                new XAttribute("Type", contact.ContactType),
                new XAttribute("Value", contact.ContactValue)
            ));
        }

        return new XElement(CbNs + "Company", elements);
    }

    private XElement BuildAddressElement(Address address)
    {
        var attrs = new List<XAttribute>
        {
            new XAttribute("Type", MapAddressType(address.AddressType))
        };

        if (!string.IsNullOrWhiteSpace(address.FullAddress))
        {
            attrs.Add(new XAttribute("FullAddress", address.FullAddress));
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(address.StreetNameAndNumber))
                attrs.Add(new XAttribute("StreetNo", address.StreetNameAndNumber));
            if (!string.IsNullOrWhiteSpace(address.PostalCode))
                attrs.Add(new XAttribute("PostalCode", address.PostalCode));
            attrs.Add(new XAttribute("City", address.City));
            attrs.Add(new XAttribute("District", address.District));
            attrs.Add(new XAttribute("Region", address.Region));
            attrs.Add(new XAttribute("Country", address.Country));
        }

        return new XElement(CbNs + "Address", attrs);
    }

    private XElement BuildApplicationElement(ApplicationData application)
    {
        var attrs = new List<XAttribute>
        {
            new XAttribute("ContractType", application.ContractType),
            new XAttribute("ContractPhase", MapContractPhase(application.ContractPhase)),
            new XAttribute("ContractRequestDate", application.ContractRequestDate.ToString("yyyy-MM-dd")),
            new XAttribute("Currency", application.Currency)
        };

        var children = new List<XElement>();

        // Based on contract type, add specific elements
        if (application.FinancedAmount.HasValue)
        {
            // For installment contracts
            children.Add(new XElement(CbNs + "Installment",
                new XAttribute("FinancedAmount", application.FinancedAmount.Value),
                application.MonthlyPaymentAmount.HasValue
                    ? new XAttribute("MonthlyPaymentAmount", application.MonthlyPaymentAmount.Value)
                    : null,
                application.DueDate.HasValue
                    ? new XAttribute("DueDate", application.DueDate.Value.ToString("yyyy-MM-dd"))
                    : null
            ));
        }

        if (application.CreditLimit.HasValue)
        {
            // For credit card/revolving credit
            children.Add(new XElement(CbNs + "CreditCard",
                new XAttribute("CreditLimit", application.CreditLimit.Value)
            ));
        }

        return new XElement(CbNs + "Application", attrs, children);
    }

    // Mapping helpers
    private string MapSubjectRole(Enums.SubjectRole role) => role switch
    {
        Enums.SubjectRole.Borrower => DomainTables.Role.Borrower,
        Enums.SubjectRole.CoBorrower => DomainTables.Role.CoBorrower,
        Enums.SubjectRole.Guarantor => DomainTables.Role.Guarantor,
        _ => DomainTables.Role.Borrower
    };

    private string MapGender(Enums.Gender gender) => gender switch
    {
        Enums.Gender.Male => DomainTables.Gender.Male,
        Enums.Gender.Female => DomainTables.Gender.Female,
        _ => DomainTables.Gender.Male
    };

    private string MapAddressType(Enums.AddressType type) => type switch
    {
        Enums.AddressType.Residential => DomainTables.AddressType.IndividualMain,
        Enums.AddressType.Registration => DomainTables.AddressType.IndividualAdditional,
        Enums.AddressType.Work => DomainTables.AddressType.CompanyMain,
        _ => DomainTables.AddressType.IndividualMain
    };

    private string MapContractPhase(Enums.ContractPhase phase) => phase switch
    {
        Enums.ContractPhase.Requested => DomainTables.ContractPhase.Requested,
        Enums.ContractPhase.Renounced => DomainTables.ContractPhase.Renounced,
        Enums.ContractPhase.Refused => DomainTables.ContractPhase.Refused,
        Enums.ContractPhase.Active => DomainTables.ContractPhase.Active,
        Enums.ContractPhase.Closed => DomainTables.ContractPhase.Closed,
        Enums.ContractPhase.ClosedInAdvance => DomainTables.ContractPhase.ClosedInAdvance,
        _ => DomainTables.ContractPhase.Requested
    };
}
