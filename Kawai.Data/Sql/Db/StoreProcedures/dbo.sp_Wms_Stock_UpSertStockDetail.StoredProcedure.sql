SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_Stock_UpSertStockDetail]
	@WarehouseCode	varchar(25),
	@AreaCode		varchar(25),
	@AddressCode	varchar(25),
	@ItemCode		varchar(25),
	@BarcodeNo		varchar(50),
	@LotNo			varchar(100),
	@QtyTrans		numeric(18,9),
	@InventoryQty	numeric(18,9),
	@SublotNo		int,
	@UserId			varchar(25)
as
begin
	if exists 
	(
		select 1 from StockDetail 
		where WarehouseCode = @WarehouseCode and AreaCode = @AreaCode and AddressCode = @AddressCode 
		and BarcodeNo = @BarcodeNo and ItemCode = @ItemCode and LotNo = @LotNo
	) 
	begin
		update StockDetail set Qty = Qty + @QtyTrans, InventoryQty = @InventoryQty, Lastupdate = getdate(), LastUser = @UserId
		where WarehouseCode = @WarehouseCode and AreaCode = @AreaCode and AddressCode = @AddressCode 
		and BarcodeNo = @BarcodeNo and ItemCode = @ItemCode and LotNo = @LotNo
		and Qty >= @QtyTrans
	end
	else 
	begin
		insert into StockDetail (WarehouseCode, AreaCode, AddressCode, BarcodeNo, ItemCode, LotNo, SublotNo, Qty, InventoryQty, RegisterDate, RegisterUser)
		values (@WarehouseCode, @AreaCode, @AddressCode, @BarcodeNo, @ItemCode, @LotNo, @SublotNo, @QtyTrans, @InventoryQty, getdate(), @UserId)
	end
end
GO
