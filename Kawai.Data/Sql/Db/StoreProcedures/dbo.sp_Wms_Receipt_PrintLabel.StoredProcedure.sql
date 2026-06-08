 
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

	declare @warehouseSubcon varchar(25), @tradeCls varchar(3) 
	select @warehouseSubcon = Subcon_WH_Code, @TradeCls = Trade_Cls from Trade_Master where Trade_Code = @SupplierCode

	if @tradeCls = '3'
	begin
		declare @tblRequirementMaterial table (PONumber varchar(100), ParentItem varchar(25), ItemCode varchar(25), BOMQty numeric(18,9), ReceiptQty numeric(18,9))
		insert into @tblRequirementMaterial
		select dtl.PONumber, dtl.ItemCode, bom.Item_Code, bom.Qty, dtl.ReceiptQty
		From BOM_Master bom 
		inner join 
		(
			select * From PartReceiptDetail where ReceiptId = @ReceiptId
		) dtl on bom.Parent_ItemCode = dtl.ItemCode

		declare @pickingNos table (PONumber varchar(100), ParentItem varchar(25), PickingNo varchar(100))
		insert into @pickingNos
		select distinct hd.PO_NO, hd.ParentItem_Code, dtl.RefNumber 
		From PartMaterialRequestDetail_PO dtl
		inner join PartMaterialRequestHeader_PO hd on dtl.RequestID = hd.RequestID
		inner join PartMaterialRequestItemDetail_PO dtli on dtl.RequestDetailID = dtli.RequestDetailID
		inner join 
		(
			select * From PartReceiptDetail where ReceiptId = @ReceiptId
		) req on hd.PO_NO = req.PONumber and hd.ParentItem_Code = req.ItemCode

		declare @tblPOItemSummaryBOM table (PONumber varchar(100), ParentItem varchar(25), QtyReceipt numeric(18,9), QtyMinCanReceipt numeric(18,9))
		insert into @tblPOItemSummaryBOM
		select res.PONumber, res.ParentItem, res.QtyReceipt, min(QtyCanReceipt) from 
		(
			select 
				req.PONumber, req.ParentItem, 
				QtyReceipt = req.ReceiptQty,
				QtyCanReceipt = floor(isnull(stok.OutstandingQty, 0) / req.BOMQty)
			from @tblRequirementMaterial req
			left join
			(
				select x.PONumber, x.ParentItem, ItemCode, sum(Qty) OutstandingQty 
				From StockDetail std
				inner join @pickingNos x on isnull(std.Picking_No, '') = x.PickingNo
				where 1=1
				and WarehouseCode = @warehouseSubcon 
				and Qty > 0 
				group by x.PONumber, x.ParentItem, ItemCode
			) stok on req.PONumber = stok.PONumber and req.ParentItem = stok.ParentItem and stok.ItemCode = req.ItemCode
		) res
		group by res.PONumber, res.ParentItem, res.QtyReceipt

		--declare @msg varchar(max)
		--if exists (select 1 from @tblPOItemSummaryBOM where QtyMinCanReceipt < QtyReceipt)
		--begin
		--	SELECT @msg = STRING_AGG(
		--		PONumber + ' (Need: ' + CAST(QtyReceipt AS VARCHAR) +
		--		', Can: ' + CAST(QtyMinCanReceipt AS VARCHAR) + ')'
		--	, '; ')
		--	FROM @tblPOItemSummaryBOM
		--	WHERE QtyMinCanReceipt < QtyReceipt
		--	RAISERROR('Material di warehouse subcon tidak mencukupi untuk PO: %s', 16, 1, @msg)
		--	return	
		--end
	end
	
	if exists (select 1 from PartReceiptDetailBarcode where ReceiptId = @ReceiptId)
	begin
		if isnull(@MustPrint, 0) = 1
		begin
			update PartReceiptDetailBarcode set PrintStatus = null, PrintDate = null, PrintUser = @UserId 
			where ReceiptId = @ReceiptId
		end

		return; 
	end

	-- ==============================================================================
	-- PERSIAPAN DATA & GENERATE NUMERATOR (DI LUAR TRANSAKSI & LOOPING)
	-- ==============================================================================
	declare @prefixFactory varchar(5) = (select PrefixGlobalBarcode from Company_Profile where Company_Code = @factoryCode)
	declare @dt varchar(8) = format(@ReceiptDate, 'yyyyMMdd')
	declare @prefixBarcode varchar(20) = @prefixFactory + 'RM' + @dt
	declare @prefixLot varchar(22) = 'L.'+@prefixFactory+'RM' + @dt

	declare @NewLot varchar(100)

	if not exists (select 1 from PartReceiptDetailBarcode where ReceiptId = @ReceiptId)
	begin
		EXEC dbo.GenerateNumerator @Prefix = @prefixLot, @LengthSequence = 4, @Result = @NewLot OUTPUT;			

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

		insert into @partReceiptDetail
		select ROW_NUMBER() over (order by a.ItemCode), a.ReceiptDetailId, '' PONumber, a.ItemCode, a.ReceiptQty, isnull(b.QtyPacking, mi.Number_Box),  mi.WH_Code
		From 
		(
			select ItemCode, MIN(Id) ReceiptDetailId , SUM(ReceiptQty) ReceiptQty 
			from PartReceiptDetail where ReceiptId = @ReceiptId   
			group by ItemCode
		) a
		left join ItemSupplierPacking b on a.ItemCode = b.ItemCode and b.SupplierCode = @SupplierCode
		inner join Item_Master mi on a.ItemCode = mi.Item_Code

		declare @TotalBarcodes int = (select isnull(sum(ceiling(ReceiptQty / QtyPacking)), 0) from @partReceiptDetail)
		declare @lastSequenceBarcode int
	
		if @TotalBarcodes > 0
		begin
			exec GenerateNumeratorBatch @Prefix = @prefixBarcode, @RowCount = @TotalBarcodes, @LastSequence = @lastSequenceBarcode OUTPUT
		end
	end

	-- ==============================================================================
	-- EKSEKUSI TRANSAKSI INSERT/UPDATE
	-- ==============================================================================
	BEGIN TRY
		BEGIN TRANSACTION PrintLabelTrans;

		declare @i int = 1
		declare @currentBarcodeSeq int = isnull(@lastSequenceBarcode, 0) + 1	 
		declare @SublotNo int = isnull((select max(SublotNo) from PartReceiptDetailBarcode where ReceiptDate = @ReceiptDate), 0)

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

				declare @NewBarcode varchar(100) = @prefixBarcode + RIGHT('0000' + CAST(@currentBarcodeSeq AS VARCHAR), 4)
				set @currentBarcodeSeq += 1
				set @SublotNo += 1

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

		-- ==============================================================================
		-- PROSES SUBCON
		-- ==============================================================================
		if @tradeCls = '3'
		begin
			declare @tblConsume table
			(
				Urutan int, PONumber varchar(100), ParentItem varchar(25), 
				RefNo varchar(50), WarehouseCode varchar(25), AreaCode varchar(25), AddressCode varchar(25), 
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
				row_number() over (order by res.Qty) Urutan, PONumber, ParentItem, RefNo, WarehouseCode, AreaCode, AddressCode, 
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
				inner join @tblRequirementMaterial b on a.PONumber = b.PONumber and a.ParentItem = b.ParentItem and a.ItemCode = b.ItemCode
			) res 
			where QtyConsume > 0

			DECLARE @InsertedConsumption TABLE (ConsumptionId bigint, PONumber varchar(100), ItemCode varchar(25))

			insert into MaterialConsumptionReceiptSubcon
			(ReceiptId, ReceiptDetailId, PONumber, ItemCode, QtyReceipt, RegisterUser, RegisterDate)
			OUTPUT INSERTED.ConsumptionId, INSERTED.PONumber, INSERTED.ItemCode INTO @InsertedConsumption (ConsumptionId, PONumber, ItemCode)
			select ReceiptId, Id, PONumber, ItemCode, ReceiptQty, @UserId, getdate() 
			from PartReceiptDetail
			where ReceiptId = @ReceiptId

			declare @newid bigint = (select SCOPE_IDENTITY())

			insert into MaterialConsumptionReceiptSubconDetail
			(ConsumptionId, BarcodeNo, ItemCode, QtyUsed, RegisterUser, RegisterDate)
			select 
				(SELECT TOP 1 ConsumptionId FROM @InsertedConsumption yy WHERE yy.PONumber = xx.PONumber and yy.ItemCode = xx.ParentItem), 
				BarcodeNo, ItemCode, QtyConsume, @UserId, getdate() 
			from @tblConsume xx


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
	
		COMMIT TRANSACTION PrintLabelTrans;
	END TRY
	BEGIN CATCH
		declare @msgErr varchar(max) = ERROR_MESSAGE()
		ROLLBACK TRANSACTION PrintLabelTrans;
		raiserror(@msgErr, 16, 1)
		RETURN;
	END CATCH
end

