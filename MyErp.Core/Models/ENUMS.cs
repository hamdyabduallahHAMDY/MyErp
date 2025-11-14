using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyErp.Core.Models;

public enum SchemeIDs
{
    TIN,
    CRN,
    MOM,
    MLS,
    _700,
    SAG,
    NAT,
    GCC,
    IQA,
    OTH
}
public enum StockReqStatus
{
    Approved = 1,
    Rejected = 2,
}
public enum BaseOrderType
{
    requestqutaion = 1,
    order = 2,
    permmision = 3,
    requestqutaionpurhcase = 4,
    orderpurhcase = 5,
    permmisionpurchase = 6
}
public enum AccountType
{
    Debit = 0,
    Credit = 1,
    Asset = 2,
    Liability = 3,
    Equity = 4,
    Revenue = 5,
    Expense = 6,

    Other
}
public enum TaxSchemeIDs
{
    VAT,

}
public enum Title
{
    [Display(Name = "???? ????")]
    M
}
public enum Status
{
    NoAction,
    Valid,
    ValidWithWarnings,
    Invalid
}
public enum StockActiontype
{
    [Display(Name = "??? ????")]
    a,
    [Display(Name = "????? ????")]
    b,
    [Display(Name = "??? ????? ??????")]
    c
}
public enum StatusReport
{
    Reported,
    NotReported,
    AcceptedWithWarnings,
}
public enum StatusCleare
{
    Cleared,
    NotCleared

}
public enum TransactionMethod
{
    [Display(Name = "???")]
    M,
    [Display(Name = "??????")]
    T,
    [Display(Name = "?????")]
    C,
}
public enum paymentmesod
{
    [Display(Name = "Cash")]
    C,
    [Display(Name = "Visa")]
    V,
    [Display(Name = "return")]
    R,
    [Display(Name = "Credit")]
    E
}
public enum PaymentMethod
{
    [Display(Name = "Cash")]
    C,
    [Display(Name = "Visa")]
    V,
    [Display(Name = "Cash with contractor")]
    CC,
    [Display(Name = "Visa with contractor")]
    VC,
    [Display(Name = "Vouchers")]
    VO,
    [Display(Name = "Promotion")]
    PR,
    [Display(Name = "Gift Card")]
    GC,
    [Display(Name = "Points")]
    P,
    [Display(Name = "Others")]
    O,
    [Display(Name = "Due")]
    D
}
public enum OrderType
{
    Sale = 0,
    Purchase = 1,
    CreditSale = 2,
    ReturnSale = 3,
    ReturnPurchase = 4,
    CreditPurchase = 5,
    Transfer = 6,
    OfferPrice = 7
    // etc.
}

public enum InvoiceType
{
    [Description(description: "?????? ???")]
    [Display(Name = "I")]
    Invoice,
    [Description(description: "????? ???")]
    [Display(Name = "C")]
    CreditNote,
    [Description(description: "????? ?????")]
    [Display(Name = "D")]
    DebitNote,
    [Description(description: "????? ???")]
    [Display(Name = "S")]
    Receipt,
    [Description(description: "????? ????? ???")]
    [Display(Name = "R")]
    ReturnReceipt,
    [Description(description: "????? ????? ??? ???? ????")]
    [Display(Name = "RWR")]
    ReturnReceiptWithoutReference,
    [Description(description: "?????? ????? ")]
    [Display(Name = "EI")]
    ExportInvoice,
    [Description(description: "????? ??? ?????")]
    [Display(Name = "EC")]
    ExportCreditNote,
    [Description(description: " ????? ????? ?????")]
    [Display(Name = "ED")]
    ExportDebitNote,
    [Description(description: "????? ?????")]
    [Display(Name = "DR")]
    DebitReceipt,
    [Description(description: "??? ?????")]
    [Display(Name = "Uk")]
    UnknownType,
}
public enum VatCategoryCode
{
    [Display(Name = "Zero rated goods | ????????? ??????? ????? ?????")]
    Z,
    [Display(Name = "Standard rate | ????????? ??????? ???????")]
    S,
    [Display(Name = "Services outside scope of tax |  Not subject to VAT | ???????? ????? ????? ???????")]
    O,
    [Display(Name = "Exempt from Tax | ????????? ???????")]
    E
}

public enum MappingDocumentType
{
    Standard,
    Simplified
}
public enum RowStatus
{
    Active,
    Delete
}
public enum OpeinginAcc
{
    [Display(Name = "d")]
    debtor,
    [Display(Name = "c")]
    creditor
}
public enum CustomerType
{
    [Display(Name = "P")]
    Person,
    [Display(Name = "B")]
    Business,
    [Display(Name = "F")]
    Foreigner,
}
public enum EntityType
{
    [Display(Name = "C")]
    Customer,
    [Display(Name = "S")]
    Supplier
}
public enum InvoiceStatus
{
    [Display(Name = "?????")]
    Valid,
    [Display(Name = "?????")]
    Submitted,
    [Display(Name = "??? ?????")]
    Invalid,
    [Display(Name = "?????")]
    Cancel,
    [Display(Name = "??????")]
    Reject,
    [Display(Name = "???? ????")]
    NotAsync,
    NotContent,
    [Display(Name = "????")]
    Sended
}

public enum OrderSanded
{
    Sanded,
    NotSent
}
public enum InventoryPermissionType
{
    ProductWithdrawal,
    ProductAddition,
    TransferRequest
}
public enum FtpType
{
    FTP,
    SFTP
}
public enum AutoSendType
{
    Import,
    ImportReceipt,
    Send,
    SendReceipt,
    Refresh,
    ErrorMail,
    ImportFTP,
    SendFromPortal,
    ImportSharedFolder
}
public enum ControlTypes
{
    Form,
    Button,
    CheckBox,
    MenuItem
}
public enum EnvironmentPortal
{
    [Display(Name = "?????????")]
    PerProduction,
    [Display(Name = "???????")]
    Production
}
public enum PaymentCodes
{
    [Description(description: "????")]
    [Display(Name = "InCash(10)")]
    InCash,
    [Description(description: "???")]
    [Display(Name = "Credit(30)")]
    Credit,
    [Description(description: "??? ?? ???? ?????")]
    [Display(Name = "PaymentToBankA/C(42)")]
    PaymentToBank,
    [Description(description: "???? ?????")]
    [Display(Name = "BankCard(48)")]
    BankCard,
    [Description(description: "????")]
    [Display(Name = "OtherFreeText(1)")]
    OtherFreeText,
}
public enum SchemeID
{
    [Description(description: "????? ??????")]
    TIN,
    [Description(description: "??? ????? ???????")]
    CRN,
    [Description(description: "???? ????? ?????? ??????? ???????? ????????")]
    MOM,
    [Description(description: "???? ????? ??????? ??????? ???????? ??????????")]
    MLS,
    [Description(description: "700 Number"), EnumValue("700")]
    SevenHundred,
    [Description(description: "???? ????? ?????????")]
    SAG,
    [Description(description: "??? ??????")]
    NAT,
    [Description(description: "???? ??????? ???????")]
    GCC,
    [Description(description: "??? ???????")]
    IQA,
    [Description(description: "??? ????????")]
    PAS,
    [Description(description: "???? ???")]
    OTH,
}
public enum IntegrationTypes
{
    DirectDatabase,
    Odoo,
    Xero,
    ERPNext,
}
[AttributeUsage(AttributeTargets.Field)]
public class EnumValueAttribute : Attribute
{
    public string Value { get; }
    public EnumValueAttribute(string value)
    {
        Value = value;
    }
}
