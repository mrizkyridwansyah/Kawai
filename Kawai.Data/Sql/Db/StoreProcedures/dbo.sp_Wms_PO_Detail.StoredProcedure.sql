SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [sp_Wms_PO_Detail]
	@PONumber varchar(50)
as
begin
	select
		a.PO_No PONumber, a.Supplier_Code SupplierCode, b.Trade_Name SupplierName, a.PO_Date PODate, 
		a.WHTo WarehouseCode, c.WH_Name WarehouseName
	FROM PurchaseOrder_Master a
	left join trade_master b on a.Supplier_Code = b.Trade_Code
	left join WareHouse_Master c on a.WHTo= c.WH_Code
	WHERE 1=1
	and a.PO_No = @PONumber	
end
GO
