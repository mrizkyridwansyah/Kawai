SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Stock_UpSertStockHeader]
	@TransDate		date,
	@RefNo			varchar(50),
	@WarehouseCode	varchar(25),
	@AreaCode		varchar(25),
	@ItemCode		varchar(25),
	@LotNo			varchar(100),
	@QtyTrans		numeric(18,9), -- INI ADALAH QTY SELISIH NYA, MAU DITAMBAH ATAU DIKURANG BERAPA
	@InventoryQty	numeric(18,9),
	@Type			varchar(2), -- 'R' -> RECEIPT, 'S' -> SUPPLY
	@UserId			varchar(25)
as
begin
	if @Type = 'R'
	begin
		if exists 
		(
			select 1 from StockHeader 
			where RefNo = @RefNo and WarehouseCode = @WarehouseCode and AreaCode = @AreaCode and ItemCode = @ItemCode and LotNo = @LotNo
		) 
		begin		
			update StockHeader 
			set 
				TMCurrent	= TMCurrent + @QtyTrans, 
				TMReceipt	= TMReceipt + @QtyTrans, 
				TMInventory = @InventoryQty,
				NMPreMonth	= NMPreMonth + @QtyTrans, 
				NMCurrent	= NMCurrent + @QtyTrans,
				LastUpdate	= getdate(),
				LastUser	= @UserId
			where RefNo = @RefNo and WarehouseCode = @WarehouseCode and AreaCode = @AreaCode and ItemCode = @ItemCode and LotNo = @LotNo
		end
		else 
		begin
			insert into StockHeader 
			(RefNo, WarehouseCode, AreaCode, ItemCode, LotNo, TMCurrent, TMReceipt, TMInventory, NMPreMonth, NMCurrent, RegisterDate, LastUpdate, LastUser )
			values 
			(@RefNo, @WarehouseCode, @AreaCode, @ItemCode, @LotNo, @QtyTrans, @QtyTrans, @InventoryQty, @QtyTrans, @QtyTrans, getdate(), getdate(), @UserId)
		end
	end
	else if @Type = 'S'
	begin
		--if @QtyTrans < 0
		--begin
		--	set @QtyTrans = @QtyTrans * (-1)
		--end

		if exists 
		(
			select 1 from StockHeader 
			where RefNo = @RefNo and WarehouseCode = @WarehouseCode and AreaCode = @AreaCode and ItemCode = @ItemCode and LotNo = @LotNo
		) 
		begin		
		PRINT 'SUPPLY'

			update StockHeader 
			set 
				TMCurrent	= ISNULL(TMCurrent, 0) - @QtyTrans, 
				TMSupply	= ISNULL(TMSupply, 0) + @QtyTrans, 
				TMInventory = @InventoryQty,
				NMPreMonth	= ISNULL(NMPreMonth, 0) - @QtyTrans, 
				NMCurrent	= ISNULL(NMCurrent, 0) - @QtyTrans,
				LastUpdate	= getdate(),
				LastUser	= @UserId
			where RefNo = @RefNo and WarehouseCode = @WarehouseCode and AreaCode = @AreaCode and ItemCode = @ItemCode and LotNo = @LotNo
		end
		else 
		begin
			insert into StockHeader 
			(RefNo, WarehouseCode, AreaCode, ItemCode, LotNo, TMCurrent, TMSupply, TMInventory, NMPreMonth, NMCurrent, RegisterDate, LastUpdate, LastUser )
			values 
			(@RefNo, @WarehouseCode, @AreaCode, @ItemCode, @LotNo, 0, @QtyTrans, @InventoryQty, 0, 0, getdate(), getdate(), @UserId)
		end
	end

end
GO
