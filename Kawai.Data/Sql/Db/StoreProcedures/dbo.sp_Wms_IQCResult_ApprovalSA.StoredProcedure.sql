



CREATE   procedure [dbo].[sp_Wms_IQCResult_ApprovalSA]
	@InspectionId bigint,
	@InspectionResult varchar(100),
	@RemarksSA varchar(max),
	@UserId varchar(25)
as
begin
	if exists (select 1 from IQC_Inspection_Header where InspectionID = @InspectionId and InspectionResultSADate is not null)
	begin
		raiserror('Data QC sudah diapprove', 16,1)
		return
	end

	if exists 
	(
		select 1 From IQC_Inspection_Header iqc
		inner join PartReceiptHeader prh on iqc.ReceiptNo = prh.ReceiptNo
		inner join PartReceiptDetailBarcode prdb on prh.Id = prdb.ReceiptId and iqc.ItemCode = prdb.ItemCode
		where iqc.InspectionID = @InspectionId and isnull(IsVerified, 0) = 0
	) 
	begin
		raiserror('Silahkan receive semua barcode terlebih dahulu!', 16,1)
		return
	end

	declare @Source varchar(50), @ReceiptId bigint, @SupplierCode varchar(25), @DNNumber varchar(50), @PONumber varchar(50), @ItemCode varchar(25), @totalNGQty numeric(18,9), @remarks varchar(max), @receiptDate date, @factoryCode varchar(25)
	select 
		@Source = Soruce, @ReceiptId = prh.Id, @ItemCode = iqch.ItemCode, @totalNGQty = isnull(TotalQtyNG, 0), @remarks = iqch.Remarks, @receiptDate = prh.ReceiptDate, @factoryCode = prh.CompanyCode, @DNNumber = prh.DNNumber, @PONumber = iqch.PO_Number, @SupplierCode = prh.SupplierCode
	from IQC_Inspection_Header iqch
	inner join PartReceiptHeader prh on iqch.ReceiptNo = prh.ReceiptNo
	where iqch.InspectionID = @InspectionId

	if @InspectionResult = 'Accepted' and @totalNGQty > 0
	begin
		declare @currentStock numeric(18,9) = 
		(
			SELECT SUM(sd.Qty) 
			FROM StockDetail sd
			INNER JOIN IQC_SamplingBarcodeDetail qc ON sd.BarcodeNo = qc.BarcodeNo
			WHERE qc.InspectionID = @InspectionId AND sd.Qty > 0
		)
		if isnull(@currentStock, 0) < @totalNGQty
		begin
			raiserror('Sisa stock kurang dari total NG!', 16,1)
			return
		end
	end

	declare @prefixFactory varchar(5) = (select PrefixGlobalBarcode from Company_Profile where Company_Code = @factoryCode)
	declare @prefixBarcode varchar(20) = @prefixFactory + 'NG' + format(@receiptDate, 'yyyyMMdd')
	DECLARE	@NewRefNo varchar(50), @prefixPallet varchar(20) = 'PLT.' + @prefixFactory + '.' + FORMAT(GETDATE(), 'yyyyMMdd') + '.'
	declare @lotNoCalcPartialNG varchar(100), @warehouseCalcPartialNG varchar(25), @NewBarcodePartialNG varchar(50)

	if @InspectionResult = 'Accepted' and @totalNGQty > 0
	begin
		EXEC dbo.GenerateNumerator @Prefix = @prefixBarcode, @LengthSequence = 4, @Result = @NewBarcodePartialNG OUTPUT;
		EXEC dbo.GenerateNumerator @Prefix = @prefixPallet, @LengthSequence = 4, @Result = @NewRefNo OUTPUT;		
	END
	ELSE IF @InspectionResult = 'Rejected'
	BEGIN
		EXEC dbo.GenerateNumerator @Prefix = @prefixPallet, @LengthSequence = 4, @Result = @NewRefNo OUTPUT;
	END

	/* 
		DISINI HARUS NYA ADA VALIDASI CEK, BARCODE2 YG ADA DI QC INI ADA YG LG DIPAKE GA DI TRANSAKSI KAYA MATERIAL REQUEST / CONSUME
		KHUSUS YG SOURCE NYA 'MATERIAL NG' AJA
	*/

    BEGIN TRY
        BEGIN TRANSACTION IQCApprovalSATrans;

		update IQC_Inspection_Header 
		set 
			InspectionResultSA = @InspectionResult, InspectionResultSAApproval = @UserId, InspectionResultSADate = getdate(), RemarksSA = @RemarksSA,
			LastUpdate = getdate(), StatusQC = 'CONFIRMED' 
		where InspectionID = @InspectionId

		declare @status varchar(50) = case when @InspectionResult = 'Accepted' then 'OK'  when @InspectionResult = 'Rejected' then 'NG' else 'HOLD' end

		declare @calcPartialNG table(Urutan int, RefNo varchar(50), WarehouseCode varchar(25), AreaCode varchar(25), AddressCode varchar(25), BarcodeNo varchar(50), LotNo varchar(100), ItemCode varchar(25), QtyAfter numeric(18,9), QtyNG numeric(18,9), InventoryQty numeric(18,9))

		declare @toWarehouse varchar(25) = (select top 1 WarehouseCode from PartReceiptDetailBarcode where ReceiptId = @ReceiptId and ItemCode = @ItemCode)

		declare @tblFullNG table 
		(
			Urutan int,
			RefNo varchar(50), 
			FromWarehouseCode varchar(50), 
			FromAreaCode varchar(25), 
			FromAddressCode varchar(25), 
			ItemCode varchar(25), 
			BarcodeNo varchar(50), 
			LotNo varchar(100), 
			SublotNo int, 
			Qty numeric(18,9),
			InventoryQty numeric(18,9)
		)
		declare @loopRef varchar(100), @loopFromWH varchar(25), @loopFromArea varchar(25), @loopFromAdddress varchar(25), 
				@loopItem varchar(25), @loopLot varchar(100), @loopQty numeric(18, 9), @loopQtyNG numeric(18, 9), @loopIVTQty numeric(18,9), @loopBarcode varchar(50), @loopSublot int

		declare @i int = 1, @transDate date = getdate()
		declare @typeNG varchar(20)


		if @Source = 'Incoming Material'
		begin		
			set @typeNG = 'Vendor'
			-- update status receipt yg dari SAMPLING SAJA! biar ketika di Material Storage ke Address diubah jadi COMPLETE dan HILANG dr ANDON RECEIVING
			update PartReceiptHeader set StatusReceipt = @status, LastUpdate = getdate(), LastUser = @UserId where Id = @ReceiptId 

			-- UPDATE Stock yg ada di receipt tersebut, QC Receiving.
			update sd 
			set 
				StatusReceipt = @status,
				StatusHoldNG = case when @status = 'OK' then '' else 'Vendor' end,
				Lastupdate = getdate(),
				LastUser = @UserId
			From StockDetail sd
			inner join 
			(
				select BarcodeNo, ItemCode, LotNo 
				From PartReceiptDetailBarcode 
				where ReceiptId = @ReceiptId and ItemCode = @ItemCode
			) qc on sd.BarcodeNo = qc.BarcodeNo and sd.ItemCode = qc.ItemCode and sd.LotNo = qc.LotNo
		end
		else if @Source = 'Material NG'
		begin
			set @typeNG = 'Process'
			-- UPDATE Stock yg ada disampling NG aja karna ini adalah NG diluar receiving
			update sd 
			set 
				StatusReceipt = @status,
				StatusHoldNG = case when @status = 'OK' then '' else 'Process' end,
				Lastupdate = getdate(),
				LastUser = @UserId
			From StockDetail sd
			inner join 
			(
				select BarcodeNG From IQC_SamplingBarcodeDetail 
				where InspectionID = @InspectionId
			) qc on sd.BarcodeNo = qc.BarcodeNG

		end

		if @InspectionResult = 'Accepted' and @totalNGQty > 0
		begin
			;WITH cteIQCNG AS (
				SELECT 
					sd.RefNo, sd.WarehouseCode, sd.AreaCode, sd.AddressCode, sd.BarcodeNo, sd.LotNo, sd.ItemCode, sd.Qty, sd.InventoryQty,
					SUM(sd.Qty) OVER (ORDER BY sd.BarcodeNo) AS running_qty
				FROM StockDetail sd
				INNER JOIN 
				(
					SELECT InspectionID, BarcodeNo = case when @Source = 'Material NG' then BarcodeNG else BarcodeNo end 
					FROM IQC_SamplingBarcodeDetail WHERE InspectionID = @InspectionId
				) qc ON sd.BarcodeNo = qc.BarcodeNo
				WHERE qc.InspectionID = @InspectionId AND sd.Qty > 0
			)
			
			insert into @calcPartialNG
			select 
				row_number() over (order by res.Qty) Urutan, RefNo, WarehouseCode, AreaCode, AddressCode, BarcodeNo, LotNo, ItemCode, 
				Qty - QtyNG, QtyNG, InventoryQty 
			From 
			(
				SELECT 
					RefNo, WarehouseCode, AreaCode, AddressCode, BarcodeNo, LotNo, ItemCode, Qty,
					CASE 
						WHEN running_qty - Qty >= @totalNGQty THEN 0
						WHEN running_qty <= @totalNGQty THEN Qty
						ELSE @totalNGQty - (running_qty - Qty)
					END AS QtyNG, InventoryQty
				FROM cteIQCNG
			) res where QtyNG > 0

			select top 1 @lotNoCalcPartialNG = LotNo, @warehouseCalcPartialNG = WarehouseCode from @calcPartialNG

			-- INSERT STOCK NG BARU
			EXEC sp_Wms_Stock_UpSertStockDetail @NewRefNo, @warehouseCalcPartialNG, 'TMP', 'TMP', @ItemCode, @NewBarcodePartialNG, @lotNoCalcPartialNG, @totalNGQty, NULL, NULL, @UserId, 'NG', @typeNG

			-- KURANGIN STOCK LAMA
			set @i = 1
			while @i <= (select count(1) from @calcPartialNG)
			begin
				select 
					@loopRef = RefNo, @loopFromWH = WarehouseCode, @loopFromArea = AreaCode, @loopFromAdddress = AddressCode, @loopItem = ItemCode, 
					@loopLot = LotNo, @loopQty = QtyAfter, @loopQtyNG = QtyNG, @loopIVTQty = InventoryQty, @loopBarcode = BarcodeNo, @loopSublot = null
				from @calcPartialNG where Urutan = @i

				exec sp_Wms_Stock_UpSertStockDetail @loopRef, @loopFromWH, @loopFromArea, @loopFromAdddress, @loopItem, @loopBarcode, @loopLot, @loopQty, @loopIVTQty, @loopSublot, @UserId, 'OK', ''

				if @loopFromWH <> @warehouseCalcPartialNG or @loopFromArea <> 'TMP'
				begin
					insert into ReceiptSupplyHistory 
					(
						[Status], ProcessMenu, RefNo, 
						WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
						QtyTrans, Remarks, ReferenceNo, LogDate, UserID
					)
					select 
						'OUT', 'IQC Approval', @loopRef, 
						@loopFromWH, @loopFromArea, @loopFromAdddress, @loopItem, @loopBarcode, @loopLot, 
						@loopQtyNG, @remarks + ' (Approval QC Accepted - NG ' + @typeNG +')', cast(@InspectionID as varchar), getdate(), @UserId

					exec sp_Wms_Stock_UpSertStockHeader @transDate, @loopRef, @loopFromWH, @loopFromArea, @loopItem, @loopLot, @loopQtyNG, null, 'S', @UserId
					EXEC sp_Wms_Stock_UpSertStockHeader @transDate, @NewRefNo, @warehouseCalcPartialNG, 'TMP', @ItemCode, @lotNoCalcPartialNG, @loopQtyNG, @loopIVTQty, 'R', @UserId

					insert into ReceiptSupplyHistory 
					(
						[Status], ProcessMenu, RefNo, 
						WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
						QtyTrans, Remarks, ReferenceNo, LogDate, UserID
					)
					select 'IN', 'IQC Approval', RefNo, 
						sd.WarehouseCode, sd.AreaCode, sd.AddressCode, sd.ItemCode, sd.BarcodeNo, sd.LotNo, 
						sd.Qty, @remarks + ' (Approval QC Accepted - NG ' + @typeNG +')', cast(@InspectionID as varchar), getdate(), @UserId
					from StockDetail sd where BarcodeNo = @NewBarcodePartialNG and Qty > 0
				end

				set @i += 1
			end

			-- INSERT STOCK NG KE BARCODE SPLIT BIAR KE PRINT
			insert into Barcode_Split(Warehouse_Code, Area_Code, Address_Code, BarcodeNo, Item_Code, Lot_No, Qty, Print_Cls, Supplier, RegisterDate, RegisterUser, SourceNG, FromWarehouse)
			values (@warehouseCalcPartialNG, 'TMP', 'TMP', @NewBarcodePartialNG, @ItemCode, @lotNoCalcPartialNG, @totalNGQty, 0, @SupplierCode, getdate(), @UserId, 1, @loopFromWH)

			-- BarcodeNGDetail UNTUK TAU ASAL DARI BARCODE NG INI DARI MANA
			insert into BarcodeNGDetail (ReceiptId, DNNumber, SupplierCode, ItemCode, PONumber, BarcodeOriginal, BarcodeNew, QtyNG)
			select @ReceiptId, @DNNumber, @SupplierCode, @ItemCode, @PONumber, BarcodeNo, @NewBarcodePartialNG, QtyNG
			from @calcPartialNG

		end
		else if @InspectionResult = 'Rejected'
		begin
			if @Source = 'Incoming Material'
			begin
				insert into @tblFullNG
				select 
					ROW_NUMBER() over (order by sd.RefNo), 
					sd.RefNo, 
					sd.WarehouseCode, sd.AreaCode, sd.AddressCode, sd.ItemCode, sd.BarcodeNo, sd.LotNo, sd.SublotNo, sd.Qty, sd.InventoryQty
				FROM StockDetail sd
				inner join 
				(
					select BarcodeNo, ItemCode, LotNo 
					From PartReceiptDetailBarcode 
					where ReceiptId = @ReceiptId and ItemCode = @ItemCode
				) qc on sd.BarcodeNo = qc.BarcodeNo and sd.ItemCode = qc.ItemCode and sd.LotNo = qc.LotNo
				where sd.Qty > 0			
			end
			else if @Source = 'Material NG'
			begin
				insert into @tblFullNG
				select 
					ROW_NUMBER() over (order by sd.RefNo), 
					sd.RefNo, 
					sd.WarehouseCode, sd.AreaCode, sd.AddressCode, sd.ItemCode, sd.BarcodeNo, sd.LotNo, sd.SublotNo, sd.Qty, sd.InventoryQty
				FROM StockDetail sd
				inner join 
				(
					select BarcodeNG From IQC_SamplingBarcodeDetail 
					where InspectionID = @InspectionId
				) qc on sd.BarcodeNo = qc.BarcodeNG
				where sd.Qty > 0			
			end

			insert into ReceiptSupplyHistory 
			(
				[Status], ProcessMenu, RefNo, 
				WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo, RefNo2,
				QtyTrans, Remarks, ReferenceNo, LogDate, UserID
			)
			select 
				'OUT', 'IQC Approval', sd.RefNo, 
				sd.FromWarehouseCode, sd.FromAreaCode, sd.FromAddressCode, sd.ItemCode, sd.BarcodeNo, sd.LotNo, @NewRefNo,
				sd.Qty, @remarks + ' (Approval QC Rejected - NG ' + @typeNG +')', cast(@InspectionID as varchar), getdate(), @UserId
			FROM @tblFullNG SD

			set @i = 1;

			while @i <= (select count(1) from @tblFullNG)
			begin
				select 
					@loopRef = RefNo, @loopFromWH = FromWarehouseCode, @loopFromArea = FromAreaCode, @loopFromAdddress = FromAddressCode, @loopItem = ItemCode, 
					@loopLot = LotNo, @loopQty = Qty, @loopIVTQty = InventoryQty, @loopBarcode = BarcodeNo, @loopSublot = SublotNo
				from @tblFullNG where Urutan = @i

				exec sp_Wms_Stock_UpSertStockDetail @loopRef, @loopFromWH, @loopFromArea, @loopFromAdddress, @loopItem, @loopBarcode, @loopLot, 0, NULL, @loopSublot, @UserId, 'NG', @typeNG
				exec sp_Wms_Stock_UpSertStockHeader @transDate, @loopRef, @loopFromWH, @loopFromArea, @loopItem, @loopLot, @loopQty, NULL, 'S', @UserId

				exec sp_Wms_Stock_UpSertStockDetail @NewRefNo, @toWarehouse, 'TMP', 'TMP', @loopItem, @loopBarcode, @loopLot, @loopQty, @loopIVTQty, @loopSublot, @UserId, 'NG', @typeNG
				exec sp_Wms_Stock_UpSertStockHeader @transDate, @NewRefNo, @toWarehouse, 'TMP', @loopItem, @loopLot, @loopQty, @loopIVTQty, 'R', @UserId

				set @i += 1
			end

			insert into ReceiptSupplyHistory 
			(
				[Status], ProcessMenu, RefNo, 
				WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo, RefNo2,
				QtyTrans, Remarks, ReferenceNo, LogDate, UserID
			)
			select 
				'IN', 'IQC Approval', @NewRefNo, 
				@toWarehouse, 'TMP', 'TMP', sd.ItemCode, sd.BarcodeNo, sd.LotNo, sd.RefNo,
				sd.Qty, @remarks + ' (Approval QC Rejected - NG ' + @typeNG +')', cast(@InspectionID as varchar), getdate(), @UserId
			FROM @tblFullNG SD
		end

	    COMMIT TRANSACTION IQCApprovalSATrans;
    END TRY
    BEGIN CATCH
		declare @msgErr varchar(max) = (select ERROR_MESSAGE())
        ROLLBACK TRANSACTION IQCApprovalSATrans;
		raiserror(@msgErr, 16, 1)
        RETURN;
    END CATCH
end

