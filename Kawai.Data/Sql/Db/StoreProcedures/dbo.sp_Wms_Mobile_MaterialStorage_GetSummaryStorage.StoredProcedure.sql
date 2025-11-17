SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [sp_Wms_Mobile_MaterialStorage_GetSummaryStorage]
	@WarehouseCode varchar(25) 
as
begin
	SELECT @WarehouseCode WarehouseCode, res.*, mi.Item_Name ItemName fROM
	(
		SELECT 
			ItemCode, LotNo, SUM(Qty) TotalQty, COUNT(BarcodeNo) TotalItem 
		FROM StockDetail
		WHERE WarehouseCode = @WarehouseCode and AreaCode = 'TMP' and AddressCode = 'TMP' and Qty > 0	
		GROUP BY ItemCode, LotNo
	) res
	LEFT JOIN Item_Master mi on res.ItemCode = mi.Item_Code
end
GO
