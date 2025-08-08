using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class Trade
{
    public string Trade_Code { get; set; }
    public string Trade_Cls { get; set; }
    public string Trade_Name { get; set; }
    public string Trade_Abbr { get; set; }
    public string Contact_Person { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string Country_Cls { get; set; }
    public string Epte_Cls { get; set; }
    public string Region_Cls { get; set; }
    public string Postal_Code { get; set; }
    public string Telephone { get; set; }
    public string Fax { get; set; }
    public string Closing_Day { get; set; }
    public string Pay_Day { get; set; }
    public string InvoicePay_Days { get; set; }
    public string Affiliate_Cls { get; set; }
    public string Insurance_Cls { get; set; }
    public string NPWP_No { get; set; }
    public string NPWP_Name { get; set; }
    public string NPWP_Address { get; set; }
    public string NPWP_City { get; set; }
    public string NPPKP_No { get; set; }
    public string Invoice_To { get; set; }
    public string PO_Cls { get; set; }
    public string Price_Condition { get; set; }
    public string POPayment_Day { get; set; }
    public string POPayment_Terms { get; set; }
    public string Transportation_Cls { get; set; }
    public string POCaseMark1 { get; set; }
    public string POCaseMark2 { get; set; }
    public string POCaseMark3 { get; set; }
    public string POCaseMark4 { get; set; }
    public string POCaseMark5 { get; set; }
    public string POMarking1 { get; set; }
    public string POMarking2 { get; set; }
    public string POMarking3 { get; set; }
    public string POMarking4 { get; set; }
    public string POMarking5 { get; set; }
    public string POMarking6 { get; set; }
    public string Subcon_WH_Code { get; set; }
    public string NG_Cls { get; set; }
    public string SAP_Code { get; set; }
    public string Type_BC { get; set; }
    public string No_Izin { get; set; }
    public string CODE_KPPBC { get; set; }
    public DateTime? NoIzin_Date { get; set; }
    public string NITKU { get; set; }
}
