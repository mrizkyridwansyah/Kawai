SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_Stock_UpSertStockMaster]
	@TransDate		date,
	@WarehouseCode	varchar(25),
	@AreaCode		varchar(25),
	@ItemCode		varchar(25),
	@LotNo			varchar(100),
	@QtyTrans		numeric(18,9),
	@InventoryQty	numeric(18,9),
	@Type			varchar(2), -- 'R' -> RECEIPT, 'S' -> SUPPLY
	@UserId			varchar(25)
as
begin
	if @Type = 'R'
	begin
		if exists 
		(
			select 1 from StockMaster 
			where WarehouseCode = @WarehouseCode and AreaCode = @AreaCode and ItemCode = @ItemCode and LotNo = @LotNo
		) 
		begin		
			update StockMaster 
			set 
				TMCurrent	= TMCurrent + @QtyTrans, 
				TMReceipt	= TMReceipt + @QtyTrans, 
				TMInventory = @InventoryQty,
				NMPreMonth	= NMPreMonth + @QtyTrans, 
				NMCurrent	= NMCurrent + @QtyTrans,
				LastUpdate	= getdate(),
				LastUser	= @UserId
			where WarehouseCode = @WarehouseCode and AreaCode = @AreaCode and ItemCode = @ItemCode and LotNo = @LotNo
		end
		else 
		begin
			insert into StockMaster 
			(WarehouseCode, AreaCode, ItemCode, LotNo, TMCurrent, TMReceipt, TMInventory, NMPreMonth, NMCurrent, RegisterDate, LastUpdate, LastUser )
			values 
			(@WarehouseCode, @AreaCode, @ItemCode, @LotNo, @QtyTrans, @QtyTrans, @InventoryQty, @QtyTrans, @QtyTrans, getdate(), getdate(), @UserId)
		end
	end
	else if @Type = 'S'
	begin
		if @QtyTrans < 0
		begin
			set @QtyTrans = @QtyTrans * (-1)
		end

		if exists 
		(
			select 1 from StockMaster 
			where WarehouseCode = @WarehouseCode and AreaCode = @AreaCode and ItemCode = @ItemCode and LotNo = @LotNo
		) 
		begin		
			update StockMaster 
			set 
				TMCurrent	= TMCurrent + @QtyTrans, 
				TMSupply	= TMSupply + @QtyTrans, 
				TMInventory = @InventoryQty,
				NMPreMonth	= NMPreMonth - @QtyTrans, 
				NMCurrent	= NMCurrent - @QtyTrans,
				LastUpdate	= getdate(),
				LastUser	= @UserId
			where WarehouseCode = @WarehouseCode and AreaCode = @AreaCode and ItemCode = @ItemCode and LotNo = @LotNo
		end
		else 
		begin
			insert into StockMaster 
			(WarehouseCode, AreaCode, ItemCode, LotNo, TMCurrent, TMSupply, TMInventory, NMPreMonth, NMCurrent, RegisterDate, LastUpdate, LastUser )
			values 
			(@WarehouseCode, @AreaCode, @ItemCode, @LotNo, 0, @QtyTrans, @InventoryQty, 0, 0, getdate(), getdate(), @UserId)
		end
	end

end
GO
