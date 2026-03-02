SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [sp_Wms_Mobile_SupplyScanRequest_GetListDetail]
	@WarehouseCode VARCHAR(100),
	@RequestNo	  VARCHAR(100),
	@ItemCode	  VARCHAR(100)
as
begin
	select LotNo,AddressCode  [Address]  , SUM(Qty) Qty , @WarehouseCode WarehouseCode , @ItemCode ItemCode , AreaCode ItemName , @RequestNo RequestNo
	from StockDetail A  
	where ItemCode = @ItemCode and Qty > 0 and isnull(A.Picking_No, '') = ''
	Group by LotNo,AddressCode ,AreaCode
end
 

GO
