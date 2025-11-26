namespace CRIF_API.Client.Enums;

/// <summary>
/// Contract phases according to CRIF documentation
/// </summary>
public enum ContractPhase
{
    /// <summary>
    /// Requested (RQ) - Credit application submitted
    /// </summary>
    Requested,

    /// <summary>
    /// Renounced (RN) - Application withdrawn by subject
    /// </summary>
    Renounced,

    /// <summary>
    /// Refused (RF) - Application refused by provider
    /// </summary>
    Refused,

    /// <summary>
    /// Active (AC) - Active credit agreement
    /// </summary>
    Active,

    /// <summary>
    /// Closed (CL) - Closed on or after expected date
    /// </summary>
    Closed,

    /// <summary>
    /// Closed in Advance (CA) - Closed before expected date
    /// </summary>
    ClosedInAdvance
}

/// <summary>
/// Role of subject in contract
/// </summary>
public enum SubjectRole
{
    /// <summary>
    /// Borrower - Single party in credit agreement
    /// </summary>
    Borrower,

    /// <summary>
    /// Co-Borrower - Multiple parties in same agreement
    /// </summary>
    CoBorrower,

    /// <summary>
    /// Guarantor - Guarantees borrower's debt
    /// </summary>
    Guarantor
}

/// <summary>
/// Company role (position in organization)
/// </summary>
public enum CompanyRole
{
    Chairman,
    ChairmanOfBoardOfDirectors,
    CEO,
    Director,
    ViceDirector,
    Shareholder,
    Member,
    MemberOfBoardOfDirectors,
    Other
}

/// <summary>
/// Subject type
/// </summary>
public enum SubjectType
{
    /// <summary>
    /// Individual person
    /// </summary>
    Individual,

    /// <summary>
    /// Legal entity / Company
    /// </summary>
    Company
}

/// <summary>
/// Contract categories
/// </summary>
public enum ContractCategory
{
    /// <summary>
    /// Installment contracts (I)
    /// </summary>
    Installment,

    /// <summary>
    /// Non-installment contracts (N)
    /// </summary>
    NonInstallment,

    /// <summary>
    /// Credit card contracts (C)
    /// </summary>
    CreditCard,

    /// <summary>
    /// Service contracts (S)
    /// </summary>
    Service
}

/// <summary>
/// Gender
/// </summary>
public enum Gender
{
    Male,
    Female,
    Unknown
}

/// <summary>
/// Marriage status
/// </summary>
public enum MarriageStatus
{
    Single,
    Married,
    Divorced,
    Widowed,
    Unknown
}

/// <summary>
/// Address type
/// </summary>
public enum AddressType
{
    Residential,
    Registration,
    Work,
    Other
}

/// <summary>
/// Enquiry type for footprint
/// </summary>
public enum EnquiryType
{
    /// <summary>
    /// New Application Enquiry
    /// </summary>
    NAE,

    /// <summary>
    /// Monitoring Enquiry
    /// </summary>
    ME
}

/// <summary>
/// Product role for different enquiry permissions
/// </summary>
public enum ProductRole
{
    /// <summary>
    /// Full credit report
    /// </summary>
    Full,

    /// <summary>
    /// Basic credit report (limited contract details)
    /// </summary>
    Basic,

    /// <summary>
    /// Application only (no credit history)
    /// </summary>
    ApplicationOnly
}

/// <summary>
/// Presentation culture for PDF output
/// </summary>
public enum PresentationCulture
{
    Russian,
    English,
    Uzbek
}
