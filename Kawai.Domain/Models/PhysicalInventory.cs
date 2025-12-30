namespace Kawai.Domain.Models;

using System;

public class StockMaster
{
    // ===== PRIMARY KEY =====
    public string Warehouse_Code { get; set; }
    public string Item_Code { get; set; }

    // ===== LAST MONTH (LM) =====
    public decimal? LM_PreMonth { get; set; }
    public decimal? LM_Receipt { get; set; }
    public decimal? LM_Supply { get; set; }
    public decimal? LM_LossReject { get; set; }
    public decimal? LM_Current { get; set; }
    public decimal? LM_Inventory { get; set; }
    public string LM_Reason { get; set; }

    // ===== THIS MONTH (TM) =====
    public decimal? TM_PreMonth { get; set; }
    public decimal? TM_Receipt { get; set; }
    public decimal? TM_Supply { get; set; }
    public decimal? TM_LossReject { get; set; }
    public decimal? TM_Current { get; set; }
    public decimal? TM_Inventory { get; set; }
    public string TM_Reason { get; set; }

    // ===== NEXT MONTH (NM) =====
    public decimal? NM_PreMonth { get; set; }
    public decimal? NM_Receipt { get; set; }
    public decimal? NM_Supply { get; set; }
    public decimal? NM_LossReject { get; set; }
    public decimal? NM_Current { get; set; }
    public decimal? NM_Inventory { get; set; }
    public string NM_Reason { get; set; }

    // ===== AUDIT =====
    public DateTime? Last_Update { get; set; }
    public string Last_User { get; set; }
    public DateTime? Register_Date { get; set; }
}
