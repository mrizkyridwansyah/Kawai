SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_Stock_UpSertStockDetail]
	@RefNo			varchar(50),
	@WarehouseCode	varchar(25),
	@AreaCode		varchar(25),
	@AddressCode	varchar(25),
	@ItemCode		varchar(25),
	@BarcodeNo		varchar(50),
	@LotNo			varchar(100),
	@QtyAfter		numeric(18,9), --PARAMETER INI ADALAH QTY AKHIR
	@InventoryQty	numeric(18,9),
	@SublotNo		int,
	@UserId			varchar(25),
	@StatusReceipt	varchar(20) = null
as
begin
	if exists 
	(
		select 1 from StockDetail 
		where RefNo = @RefNo and WarehouseCode = @WarehouseCode and AreaCode = @AreaCode and AddressCode = @AddressCode 
		and BarcodeNo = @BarcodeNo and ItemCode = @ItemCode and LotNo = @LotNo
	) 
	begin
		update StockDetail set Qty = @QtyAfter, InventoryQty = @InventoryQty, Lastupdate = getdate(), LastUser = @UserId
		where RefNo = @RefNo and WarehouseCode = @WarehouseCode and AreaCode = @AreaCode and AddressCode = @AddressCode 
		and BarcodeNo = @BarcodeNo and ItemCode = @ItemCode and LotNo = @LotNo
		--and Qty >= @QtyAfter
	end
	else 
	begin
		insert into StockDetail (RefNo, WarehouseCode, AreaCode, AddressCode, BarcodeNo, ItemCode, LotNo, SublotNo, Qty, InventoryQty, StatusReceipt, RegisterDate, RegisterUser)
		values (@RefNo, @WarehouseCode, @AreaCode, @AddressCode, @BarcodeNo, @ItemCode, @LotNo, @SublotNo, @QtyAfter, @InventoryQty, @StatusReceipt, getdate(), @UserId)
	end
end
GO
