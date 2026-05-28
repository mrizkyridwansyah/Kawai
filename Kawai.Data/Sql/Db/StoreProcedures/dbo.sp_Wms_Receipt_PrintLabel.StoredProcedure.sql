 
CREATE PROCEDURE [dbo].[sp_Wms_Receipt_PrintLabel]
	@ReceiptId		varchar(50),
	@MustPrint	bit = null,
	@UserId			varchar(25)
as
begin
	if not exists (select 1 from PartReceiptHeader where Id = @ReceiptId)
	begin
		raiserror('Data Receipt tidak ditemukan!', 16, 1)
		return
	end

	if not exists 
	(
		select * From 
		(
			select * from PartReceiptHeader where Id = @ReceiptId
		) a 
		inner join 
		(
			select * from SS_UserFactoryPrivilege where UserID = @UserId and isnull(AllowAccess, 0) = 1
		) b on a.CompanyCode = b.FactoryCode
	)
	begin
		raiserror('User tidak memiliki hak akses ke factory ini!', 16, 1)
		return
	end

	declare @ReceiptDate date, @SupplierCode varchar(50), @factoryCode varchar(25), @ReceiptNo varchar(100)
	select @ReceiptNo = ReceiptNo, @ReceiptDate = ReceiptDate, @SupplierCode = SupplierCode, @factoryCode = CompanyCode 
	from PartReceiptHeader where Id = @ReceiptId

	if exists (select 1 from PartReceiptDetailBarcode where ReceiptId = @ReceiptId)
	begin
		if isnull(@MustPrint, 0) = 1
		begin
			update PartReceiptDetailBarcode set PrintStatus = null, PrintDate = null, PrintUser = @UserId 
			where ReceiptId = @ReceiptId
		end
	end
	else
	begin
		declare @prefixFactory varchar(5) = (select PrefixGlobalBarcode from Company_Profile where Company_Code = @factoryCode)

		declare @dt varchar(8) = format(@ReceiptDate, 'yyyyMMdd')
		declare @prefixBarcode varchar(20) = @prefixFactory + 'RM' + @dt
		declare @prefixLot varchar(22) = 'L.'+@prefixFactory+'RM' + @dt

		declare @NewLot varchar(100)
		EXEC dbo.GenerateNumerator @Prefix = @prefixLot, @LengthSequence = 4, @Result = @NewLot OUTPUT;			

		declare @i int = 1
		declare @partReceiptDetail table 
		(
			[Urutan] int,
			[Id] bigint,
			[PONumber] [varchar](50) ,
			[ItemCode] [varchar](25) ,
			[ReceiptQty] [numeric](18,9),
			[QtyPacking] [numeric](18, 9),
			[WarehouseCode] [varchar](25)
		)
	
		--insert into @partReceiptDetail
		--select ROW_NUMBER() over (order by Id), Id, a.PONumber, a.ItemCode, a.ReceiptQty, isnull(b.QtyPacking, mi.Number_Box), isnull(po.WHTo, mi.WH_Code)
		--From PartReceiptDetail a
		--left join ItemSupplierPacking b on a.ItemCode = b.ItemCode and b.SupplierCode = @SupplierCode
		--left join PurchaseOrder_Master po on a.PONumber = po.PO_No
		--inner join Item_Master mi on a.ItemCode = mi.Item_Code
		--where ReceiptId = @ReceiptId

		insert into @partReceiptDetail
		select ROW_NUMBER() over (order by a.ItemCode), ROW_NUMBER() over (order by a.ItemCode), '' PONumber, a.ItemCode, a.ReceiptQty, isnull(b.QtyPacking, mi.Number_Box),  mi.WH_Code
		From (select ItemCode , SUM(ReceiptQty) ReceiptQty from PartReceiptDetail where ReceiptId = @ReceiptId   group by ItemCode) a
		left join ItemSupplierPacking b on a.ItemCode = b.ItemCode and b.SupplierCode = @SupplierCode
		inner join Item_Master mi on a.ItemCode = mi.Item_Code
	 

		while @i <= (select count(1) from @partReceiptDetail)
		begin
			declare 
				@ReceiptDetailId bigint, @PONumber varchar(50), @ItemCode varchar(25), 
				@ReceiptQty numeric(18,9), @QtyPacking numeric(18,9), @WarehouseCode varchar(25)

			select 
				@ReceiptDetailId = Id, @PONumber = PONumber, @ItemCode = ItemCode,
				@ReceiptQty = ReceiptQty, @QtyPacking = QtyPacking, @WarehouseCode = WarehouseCode
			From @partReceiptDetail where Urutan = @i

			while @ReceiptQty > 0
			begin
				declare @tempQty numeric(18,9) = @QtyPacking

				declare @NewBarcode varchar(100) 
				EXEC dbo.GenerateNumerator @Prefix = @prefixBarcode, @LengthSequence = 4, @Result = @NewBarcode OUTPUT;

				declare @SublotNo int = isnull((select max(SublotNo) from PartReceiptDetailBarcode where ReceiptDate = @ReceiptDate), 0) + 1

				if @ReceiptQty < @QtyPacking
				begin
					set @tempQty = @ReceiptQty
				end

				if isnull(@MustPrint, 0) = 1
				begin
					insert into PartReceiptDetailBarcode (ReceiptDetailId, ReceiptId, ReceiptDate, PONumber,ItemCode, BarcodeNo, LotNo, SublotNo, Qty, WarehouseCode)
					values (@ReceiptDetailId, @ReceiptId, @ReceiptDate, isnull(@PONumber, ''), @ItemCode, @NewBarcode, @NewLot, @SublotNo, @tempQty, @WarehouseCode)
				end
				else
				begin
					insert into PartReceiptDetailBarcode 
					(ReceiptDetailId, ReceiptId, ReceiptDate, PONumber,ItemCode, BarcodeNo, LotNo, SublotNo, Qty, WarehouseCode, PrintDate, PrintStatus, PrintUser)
					values 
					(@ReceiptDetailId, @ReceiptId, @ReceiptDate, isnull(@PONumber, ''), @ItemCode, @NewBarcode, @NewLot, @SublotNo, @tempQty, @WarehouseCode, getdate(), 1, @UserId)
				end

				set @ReceiptQty -= @tempQty
			end

			set @i += 1
		end

		UPDATE Part_Receipt SET Lot_No = @NewLot, Remarks = 'Update WMS_ReceiptPrintLabel' WHERE RefWMSReceiptId = @ReceiptId
	end

	declare @warehouseSubcon varchar(25), @tradeCls varchar(3) 
	select @warehouseSubcon = Subcon_WH_Code, @TradeCls = Trade_Cls from Trade_Master where Trade_Code = @SupplierCode

	if @tradeCls = '3'
	begin
		declare @Details table (PONumber varchar(50), ItemCode varchar(25), ReceiptQty numeric(18,9))
		insert into @Details
		select PONumber, ItemCode, ReceiptQty 
		From PartReceiptDetail
		where ReceiptId = @ReceiptId

		declare @tblRequirementMaterial table (PONumber varchar(100), ParentItem varchar(25), ItemCode varchar(25), BOMQty numeric(18,9), ReceiptQty numeric(18,9))
		insert into @tblRequirementMaterial
		select dtl.PONumber, dtl.ItemCode, bom.Item_Code, bom.Qty, dtl.ReceiptQty
		From BOM_Master bom 
		inner join @Details dtl on bom.Parent_ItemCode = dtl.ItemCode

		declare @pickingNos table (PONumber varchar(100), ParentItem varchar(25), PickingNo varchar(100))
		insert into @pickingNos
		select distinct hd.PO_NO, hd.ParentItem_Code, dtl.RefNumber 
		From PartMaterialRequestDetail_PO dtl
		inner join PartMaterialRequestHeader_PO hd on dtl.RequestID = hd.RequestID
		inner join PartMaterialRequestItemDetail_PO dtli on dtl.RequestDetailID = dtli.RequestDetailID
		inner join @Details req on hd.PO_NO = req.PONumber

		declare @tblConsume table
		(
			Urutan int, RefNo varchar(50), WarehouseCode varchar(25), AreaCode varchar(25), AddressCode varchar(25), 
			BarcodeNo varchar(50), LotNo varchar(100), ItemCode varchar(25), QtyAfter numeric(18,9), QtyConsume numeric(18,9), InventoryQty numeric(18,9)
		)

		;with cteSubcon as 
		(
			select 
				x.PONumber, x.ParentItem, sd.RefNo, sd.WarehouseCode, sd.AreaCode, sd.AddressCode, sd.BarcodeNo, sd.LotNo, sd.ItemCode, sd.Qty, sd.InventoryQty,
				SUM(sd.Qty) OVER (ORDER BY BarcodeNo) AS running_qty
			From StockDetail sd
			inner join @pickingNos x on isnull(sd.Picking_No, '') = x.PickingNo
			where 1=1
			and WarehouseCode = @warehouseSubcon 
			and Qty > 0 
		)

		insert into @tblConsume
		select 
			row_number() over (order by res.Qty) Urutan, RefNo, WarehouseCode, AreaCode, AddressCode, 
			BarcodeNo, LotNo, ItemCode, QtyAfter = (Qty - QtyConsume), QtyConsume, InventoryQty 
		From 
		(
			SELECT 
				a.*,
				CASE 
					WHEN a.running_qty - a.Qty >= b.ReceiptQty THEN 0
					WHEN a.running_qty <= b.ReceiptQty THEN Qty
					ELSE b.ReceiptQty - (a.running_qty - a.Qty)
				END AS QtyConsume
			FROM cteSubcon a
			inner join @Details b on a.PONumber = b.PONumber and a.ParentItem = b.ItemCode 
		) res 
		where QtyConsume > 0

		insert into MaterialConsumptionReceiptSubcon
		(ReceiptId, ReceiptDetailId, PONumber, ItemCode, QtyReceipt, RegisterUser, RegisterDate)
		select ReceiptId, Id, PONumber, ItemCode, ReceiptQty, @UserId, getdate() 
		from PartReceiptDetail
		where ReceiptId = @ReceiptId

		declare @newid bigint = (select SCOPE_IDENTITY())

		insert into MaterialConsumptionReceiptSubconDetail
		(ConsumptionId, BarcodeNo, ItemCode, QtyUsed, RegisterUser, RegisterDate)
		select @newid, BarcodeNo, ItemCode, QtyConsume, @UserId, getdate() from @tblConsume

		insert into ReceiptSupplyHistory 
		(
			[Status], ProcessMenu, RefNo, 
			WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
			QtyTrans, Remarks, ReferenceNo, LogDate, UserID
		)
		select 
			'OUT', 'Consume Subcon Material', c.RefNo, 
			c.WarehouseCode, c.AreaCode, c.AddressCode, c.ItemCode, c.BarcodeNo, c.LotNo, 
			c.QtyConsume, 'Consume Subcon Material From Receipt : ', @ReceiptNo, getdate(), @UserId
		FROM @tblConsume c

		update sd
			set sd.Qty = x.QtyAfter, Lastupdate = getdate(), LastUser = @UserId
		From StockDetail sd
		inner join @tblConsume x 
		on sd.RefNo = x.RefNo and sd.WarehouseCode = x.WarehouseCode and sd.AreaCode = x.AreaCode and sd.AddressCode = x.AddressCode
		and sd.BarcodeNo = x.BarcodeNo and sd.ItemCode = x.ItemCode and sd.LotNo = x.LotNo

		update sh
			set 
				sh.TMSupply += TotalSupply, sh.TMCurrent -= TotalSupply, sh.NMPreMonth -= TotalSupply, sh.NMCurrent -= TotalSupply, 
				Lastupdate = getdate(), LastUser = @UserId
		From StockHeader sh
		inner join 
		(	
			select RefNo, WarehouseCode, AreaCode, ItemCode, LotNo, sum(QtyConsume) TotalSupply From @tblConsume 
			group by RefNo, WarehouseCode, AreaCode, ItemCode, LotNo
		) x 
		on sh.RefNo = x.RefNo and sh.WarehouseCode = x.WarehouseCode and sh.AreaCode = x.AreaCode and sh.ItemCode = x.ItemCode and sh.LotNo = x.LotNo

	end
	
end

