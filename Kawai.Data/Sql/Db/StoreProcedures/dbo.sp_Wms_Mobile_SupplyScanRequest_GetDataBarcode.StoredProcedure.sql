SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [sp_Wms_Mobile_SupplyScanRequest_GetDataBarcode]
	@BarcodeNo varchar(100) 
as
begin
	if not exists (select 1 from StockDetail where BarcodeNo = @BarcodeNo and Qty > 0)
	begin
		raiserror('Data Stock tidak ditemukan!', 16,1)
		return
	end

	if not exists (select 1 from StockDetail where BarcodeNo = @BarcodeNo and Qty > 0 and StatusReceipt = 'OK')
	begin
		raiserror('Status Stock belum OK!', 16,1)
		return
	end
	
	SELECT  sd.WarehouseCode, sd.BarcodeNo, Line_Code LineCode, '' RequestNo, '' ProductionDate, LotNo, vc.[Description] UnitDesc,
			sd.ItemCode, mi.Item_Name ItemName,  sd.Qty PlanQty, 0 QtyScan
	FROM StockDetail sd
	left join Item_Master mi on sd.ItemCode = mi.Item_Code
	left join vw_Cls vc on TypeData = 'Unit_Cls' and mi.Unit_Cls =  vc.ClsCode
	WHERE sd.BarcodeNo = @BarcodeNo and sd.Qty > 0
end
GO
