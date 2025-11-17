SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [sp_Wms_Mobile_MaterialStorage_GetListDetail]
	@WarehouseCode varchar(25),
	@ItemCode varchar(25),
	@LotNo varchar(25)
as
begin
	SELECT 
		sd.RefNo, sd.WarehouseCode, sd.BarcodeNo, sd.ItemCode, mi.Item_Name ItemName, sd.LotNo, sd.SublotNo, sd.Qty
	FROM StockDetail sd
	left join Item_Master mi on sd.ItemCode = mi.Item_Code
	WHERE WarehouseCode = @WarehouseCode 
	and AreaCode = 'TMP' 
	and AddressCode = 'TMP' 
	and ItemCode = @ItemCode 
	and LotNo = @LotNo 
	and Qty > 0		
end
GO
