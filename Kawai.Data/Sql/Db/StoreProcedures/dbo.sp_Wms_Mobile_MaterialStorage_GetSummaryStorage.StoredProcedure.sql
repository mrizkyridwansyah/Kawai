SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [sp_Wms_Mobile_MaterialStorage_GetSummaryStorage]
	@WarehouseCode varchar(25),
	@BarcodeNo varchar(50)
as
begin
	declare @refNo varchar(100) = (select top 1 RefNo from StockDetail where BarcodeNo = @BarcodeNo and Qty > 0)
	SELECT @WarehouseCode WarehouseCode, res.*, mi.Item_Name ItemName fROM
	(
		SELECT 
			ItemCode, LotNo, SUM(Qty) TotalQty, COUNT(BarcodeNo) TotalItem 
		FROM StockDetail
		WHERE RefNo = @refNo and WarehouseCode = @WarehouseCode and AreaCode = 'TMP' and AddressCode = 'TMP' and Qty > 0	
		AND StatusReceipt = 'OK'
		GROUP BY ItemCode, LotNo
	) res
	LEFT JOIN Item_Master mi on res.ItemCode = mi.Item_Code
end
GO
