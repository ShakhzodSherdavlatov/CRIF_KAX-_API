namespace CRIF_API.Client.Constants;

/// <summary>
/// Domain tables from CRIF Manual Section 6
/// These are the codes used in CRIF API requests/responses
/// </summary>
public static class DomainTables
{
    /// <summary>
    /// Role of subject in contract - RoleDomain
    /// </summary>
    public static class Role
    {
        public const string Borrower = "B";
        public const string CoBorrower = "C";
        public const string Guarantor = "G";
    }

    /// <summary>
    /// Company role (position) - CompanyRoleDomain
    /// </summary>
    public static class CompanyRole
    {
        public const string Chairman = "1";
        public const string ChairmanOfBoardOfDirectors = "2";
        public const string CEO = "3";
        public const string Director = "4";
        public const string DeputyDirector = "5";
        public const string Shareholder = "6";
        public const string Member = "7";
        public const string BoardMember = "8";
        public const string Other = "9";
    }

    /// <summary>
    /// Gender - GenderDomain
    /// </summary>
    public static class Gender
    {
        public const string Male = "M";
        public const string Female = "F";
    }

    /// <summary>
    /// Address types - AddressTypeDomain
    /// </summary>
    public static class AddressType
    {
        public const string IndividualMain = "MI";
        public const string IndividualAdditional = "AI";
        public const string CompanyMain = "MT";
        public const string CompanyAdditional = "AT";
    }

    /// <summary>
    /// Identification code types - IdentificationTypeDomain
    /// </summary>
    public static class IdentificationType
    {
        public const string IndividualPINFL = "1"; // PINFL физ. лица
        public const string IndividualTIN = "2";   // ИНН физ. лица
        public const string CompanyTIN = "3";      // ИНН юр. лица
        public const string EntrepreneurPINFL = "4"; // ПИНФЛ ИП
    }

    /// <summary>
    /// ID document types - IDTypeDomain
    /// </summary>
    public static class IDType
    {
        public const string PassportUZ = "1";      // Паспорт гражданина РУз
        public const string ForeignPassport = "2"; // Паспорт иностранного гражданина
        public const string DriverLicense = "3";   // Водительские права
        public const string ResidencePermit = "4"; // Вид на жительство
        public const string MilitaryID = "5";      // Воинское удостоверение
        public const string VoterID = "6";         // ID избирателя
        public const string IDCard = "7";          // ID-карта
        public const string BirthCertificate = "8"; // Свидетельство о рождении
        public const string ServiceID = "9";       // Служебное удостоверение
        public const string Other = "0";
    }

    /// <summary>
    /// Contact types - ContactTypeDomain
    /// </summary>
    public static class ContactType
    {
        public const string Phone = "1";      // Телефон
        public const string Mobile = "2";     // Мобильный телефон
        public const string Email = "3";      // Электронная почта
    }

    /// <summary>
    /// Marital status - MaritalStatusDomain
    /// </summary>
    public static class MaritalStatus
    {
        public const string Single = "1";    // Не женат/не замужем
        public const string Married = "2";   // Женат/замужем
        public const string Divorced = "3";  // Разведен/разведена
        public const string Widowed = "4";   // Вдова/вдовец
    }

    /// <summary>
    /// Occupation/employment status - OccupationStatusDomain
    /// </summary>
    public static class OccupationStatus
    {
        public const string Employed = "1";    // Работающий
        public const string Unemployed = "2";  // Безработный
    }

    /// <summary>
    /// Contract phases - ContractPhaseDomain
    /// </summary>
    public static class ContractPhase
    {
        public const string Requested = "RQ";   // Запрошенный
        public const string Renounced = "RN";   // Отозванный
        public const string Refused = "RF";     // Отказан
        public const string Active = "AC";      // Активный
        public const string Closed = "CL";      // Закрытый
        public const string ClosedInAdvance = "CA"; // Закрыт заранее
    }

    /// <summary>
    /// Payment periodicity - PaymentPeriodicityDomain
    /// </summary>
    public static class PaymentPeriodicity
    {
        public const string Daily = "D";       // Ежедневно - 1 день
        public const string Weekly = "W";      // Еженедельно - 7 дней
        public const string Fortnightly = "F"; // Двухнедельно - 15 дней
        public const string Monthly = "M";     // Ежемесячно - 30 дней
        public const string Bimonthly = "B";   // Раз в два месяца - 60 дней
        public const string Quarterly = "Q";   // Ежеквартально - 90 дней
        public const string Trimester = "T";   // Трёхмесячно - 120 дней
        public const string FiveMonthly = "C"; // Каждые 5 месяцев - 150 дней
        public const string SixMonthly = "S";  // Каждые 6 месяцев - 180 дней
        public const string Yearly = "Y";      // Ежегодно - 360 дней
        public const string Irregular = "I";   // Нерегулярно
    }

    /// <summary>
    /// Number of employees - NoOfEmployeesDomain
    /// </summary>
    public static class NumberOfEmployees
    {
        public const string LessThan10 = "01";      // <= 9
        public const string From10To49 = "02";      // 10-49
        public const string From50To249 = "03";     // 50-249
        public const string MoreThan249 = "04";     // > 249
    }

    /// <summary>
    /// Contract types - ContractTypeDomain (MOST IMPORTANT!)
    /// Format: Category: I=Installment, N=Non-installment, C=Credit card, S=Service
    /// </summary>
    public static class ContractType
    {
        // Installment contracts (I)
        public const string CreditWithoutLine = "20";              // Кредит без открытия линии
        public const string CreditIssuedWithoutLine = "21";        // Кредиты выданные без линии
        public const string SyndicatedCredit = "23";               // Синдицированный кредит
        public const string MortgageCredit = "24";                 // Ипотечный кредит
        public const string Microcredit = "25";                    // Микрокредит
        public const string Leasing = "26";                        // Лизинг
        public const string ConsumerCredit = "30";                 // Потребительский кредит
        public const string Microleasing = "31";                   // Микролизинг
        public const string Microloan = "32";                      // Микрозаем
        public const string BankInitiatedCredit = "33";            // Кредит по инициативе банка
        public const string AutoCredit = "34";                     // Автокредит
        public const string ExpressCredit = "36";                  // Экспресс кредит
        public const string EducationalCredit = "59";              // Образовательный кредит
        public const string OnlineCredit = "61";                   // Онлайн кредит
        public const string CreditWithLine = "50";                 // Кредит с открытием линии
        public const string CreditIssuedWithLine = "51";           // Кредиты выданные с линией
        public const string SyndicatedCreditWithLine = "52";       // Синдицированный кредит с линией
        public const string CurrentAccountCredit = "62";           // Кредит по контокоррентному счету
        public const string MicrocreditWithLine = "55";            // Микрокредит с открытием линии
        public const string BankInitiatedCreditWithLine = "56";    // Кредит по инициативе банка с линией
        public const string BuyNowPayLater = "90";                 // Buy Now, Pay Later (BNPL)

        // Non-installment contracts (N)
        public const string MicroloanWithLine = "58";              // Микрозаем с открытием линии
        public const string Factoring = "28";                      // Факторинг
        public const string Forfaiting = "29";                     // Форфейтинг
        public const string BNPLCreditLine = "91";                 // BNPL кредитная линия

        // Credit card contracts (C)
        public const string Overdraft = "53";                      // Овердрафт
        public const string OverdraftCard = "54";                  // Овердрафт по картам
        public const string CreditCard = "35";                     // Кредитная карта
        public const string CreditCardWithLine = "60";             // Кредитная карта с линией
        public const string RevolvingCredit = "57";                // Возобновляемый кредит

        // Service contracts (S)
        public const string WiredPhone = "80";                     // Проводная связь
        public const string MobilePhone = "81";                    // Мобильная связь
        public const string WaterSupply = "82";                    // Водоснабжение
        public const string GasSupply = "83";                      // Газоснабжение
        public const string ElectricSupply = "84";                 // Электроснабжение
        public const string SatelliteTV = "85";                    // Спутниковое ТВ
        public const string CableTV = "86";                        // Кабельное ТВ
        public const string Internet = "87";                       // Интернет
        public const string Insurance = "88";                      // Страхование
    }

    /// <summary>
    /// Legal forms - LegalFormDomain
    /// Organizational-legal forms for companies
    /// </summary>
    public static class LegalForm
    {
        // Commercial organizations
        public const string CommercialOrganization = "10";
        public const string PrivateEnterprise = "11";
        public const string Enterprise = "12";
        public const string StateEnterprise = "13";
        public const string PublicEnterprise = "14";
        public const string CooperativeEnterprise = "15";
        public const string MahallaEnterprise = "16";
        public const string JointStockCompany = "17";
        public const string StateJointStock = "18";
        public const string LimitedLiability = "19";
        public const string FullPartnership = "20";
        public const string ProductionCooperative = "21";
        public const string FarmHousehold = "22";
        public const string UnitaryEnterprise = "23";
        public const string Association = "24";
        public const string Concern = "25";
        public const string JointStockUnion = "26";
        public const string MixedPartnershipUnion = "27";
        public const string FullPartnershipUnion = "28";
        public const string LeasingCompany = "29";
        public const string Corporation = "30";
        public const string HoldingCompany = "31";
        public const string InvestmentFund = "32";
        public const string OtherCommercial = "33";

        // Non-commercial organizations
        public const string NonCommercialOrganization = "34";
        public const string StateInstitution = "35";
        public const string SelfGovernmentInstitution = "36";
        public const string PublicAssociation = "37";
        public const string PublicInstitution = "38";
        public const string UnionOfSocieties = "39";
        public const string ConsumerUnion = "40";
        public const string CooperativeInstitution = "41";
        public const string FinancialIndustrialGroup = "42";
        public const string OtherNonCommercial = "43";
        public const string FamilyEnterprise = "44";

        // Legal forms for commercial entities
        public const string PrivateEnterpriseEntity = "100";
        public const string FamilyEnterpriseEntity = "110";
        public const string FarmEntity = "120";
        public const string DehkanHousehold = "130";
        public const string EconomicPartnership = "151";
        public const string LimitedLiabilityEntity = "152";
        public const string CommercialOther = "153";
        public const string JointStockEntity = "160";
        public const string UnitaryEnterpriseEntity = "170";
        public const string ProductionCooperativeEntity = "190";

        // Legal forms for non-commercial entities
        public const string PublicAssociationEntity = "200";
        public const string SelfGovernment = "210";
        public const string ConsumerCooperative = "220";
        public const string PrivateHousingPartnership = "230";
        public const string PublicFund = "240";
        public const string LegalEntitiesUnion = "250";
        public const string Institution = "260";
        public const string NonCommercialOther = "270";

        // Separate divisions (not legal entities)
        public const string Branch = "300";
        public const string Representative = "310";
        public const string SeparateDivision = "330";

        // Individual entrepreneur forms
        public const string DehkanWithoutLegalEntity = "400";
        public const string IndividualEntrepreneur = "410";
        public const string FamilyEntrepreneurship = "420";
    }

    /// <summary>
    /// Currency codes
    /// </summary>
    public static class Currency
    {
        public const string UZS = "UZS"; // Uzbekistan Som (only accepted currency)
    }

    /// <summary>
    /// Country codes (ISO 3166-1 alpha-2)
    /// </summary>
    public static class Country
    {
        public const string Uzbekistan = "UZ";
    }
}
