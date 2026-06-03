CREATE   PROCEDURE [dbo].[sp_Wms_Mobile_MaterialNG_SaveNG]
	@BarcodeNo varchar(100),
	@QtyNG numeric(18,9),
	@UserId varchar(25)
as
begin
	if not exists (select 1 from PartReceiptDetailBarcode where BarcodeNo = @BarcodeNo and isnull(IsVerified, 0) = 1)
	begin
		raiserror('Data Barcode belum diterima!', 16,1)
		return
	end

	if not exists (select 1 from StockDetail where BarcodeNo = @BarcodeNo and StatusReceipt = 'OK')
	begin
		raiserror('Status Barcode belum OK', 16, 1)
		return
	end

	if not exists (select 1 from StockDetail where BarcodeNo = @BarcodeNo and StatusReceipt = 'OK' and Qty > 0)
	begin
		raiserror('Stock sudah habis!', 16, 1)
		return
	end

	if exists 
	(
		select 1 
		from StockDetail 
		where BarcodeNo = @BarcodeNo
		and Qty > 0 
		and WarehouseCode in 
		(
			select Subcon_WH_Code from Trade_Master where Trade_Cls = '3'
		) 
	)
	begin
		raiserror('Lokasi stock berada di warehouse subcon!', 16,1)
		return
	end

	declare @ReceiptId bigint, @DNNumber varchar(50), @ReceiptNo varchar(100), @ItemCode varchar(25), @factoryCode varchar(25), @SupplierCode varchar(25)
	select @ReceiptId = pr.ReceiptId, @DNNumber = prh.DNNumber, @ReceiptNo = prh.ReceiptNo, @ItemCode = pr.ItemCode, @factoryCode = prh.CompanyCode, @SupplierCode = prh.SupplierCode
	From PartReceiptDetailBarcode pr
	inner join PartReceiptHeader prh on prh.Id = pr.ReceiptId
	where BarcodeNo = @BarcodeNo

	declare @refNo varchar(50), @lotNo varchar(100), @warehouse varchar(25), @area varchar(25), @address varchar(25), @Qty numeric(18,9), @InventoryQty numeric(18,9)
	select @refNo = RefNo, @lotNo = LotNo, @warehouse = WarehouseCode, @area = AreaCode, @address = AddressCode, @Qty = Qty, @InventoryQty = InventoryQty
	From StockDetail where BarcodeNo = @BarcodeNo and Qty > 0

	IF @QtyNG > @Qty
	BEGIN
		raiserror('Qty NG tidak boleh melebihi Qty Stock!', 16,1)
		return
	END

	if exists 
	(
		select 1 from IQC_SamplingBarcodeDetail dtl
		inner join IQC_Inspection_Header hd on dtl.InspectionID = hd.InspectionID 
		where hd.ReceiptNo = @ReceiptNo 
		and hd.ItemCode = @ItemCode 
		and hd.Soruce = 'Material NG' 
		and hd.StatusQC = 'CONFIRMED'
	)
	begin
		raiserror('Material NG dari DN barcode ini sudah diconfirm!', 16,1)
		return
	end

	declare @InspectionId bigint = (select InspectionID from IQC_Inspection_Header where ReceiptNo = @ReceiptNo and ItemCode = @ItemCode and Soruce = 'Material NG')

	declare @transDate date = getdate()
	declare @prefixFactory varchar(5) = (select PrefixGlobalBarcode from Company_Profile where Company_Code = @factoryCode)
	declare @prefixBarcode varchar(20) = @prefixFactory + 'NG' + format(getdate(), 'yyyyMMdd')
	declare @prefixPallet varchar(20) = 'PLT.' + FORMAT(GETDATE(), 'yyyyMMdd') + '.'
	DECLARE	@NewRefNo varchar(50), @NewBarcodePartialNG varchar(50)

	begin transaction ngTransaction
	begin try
		if not exists (select 1 from IQC_Inspection_Header where ReceiptNo = @ReceiptNo and ItemCode = @ItemCode and Soruce = 'Material NG')
		begin
			insert into IQC_Inspection_Header 
			(
				PO_Number
				, ReceiptNo
				, SupplierCode
				, ItemCode
				, ItemName
				, InspectionDate
				, InspectorID
				, InspectionResult
				, Remarks
				, RegisterDate
				, LastUpdate
				, TotalQtySample
				, Soruce
				, StatusQC
			)
			select 
				a.PONumber,
				b.ReceiptNo,
				b.SupplierCode,
				a.ItemCode,
				mi.Item_Name,
				getdate(),
				@UserId,
				null,
				'',
				getdate(),
				null,
				0,
				'Material NG',
				'NEW'
			From PartReceiptDetailBarcode a
			inner join PartReceiptHeader b on a.ReceiptId = b.Id
			inner join Item_Master mi on a.ItemCode = mi.Item_Code
			where ReceiptId = @ReceiptId and BarcodeNo = @BarcodeNo

			SET @InspectionId = (select SCOPE_IDENTITY())
		end

		if not exists (select 1 from IQC_SamplingBarcodeDetail where InspectionID = @InspectionId and BarcodeNo = @BarcodeNo)
		begin
			-- PARTIAL NG			if @QtyNG < @Qty
			begin
				EXEC dbo.GenerateNumerator @Prefix = @prefixBarcode, @LengthSequence = 4, @Result = @NewBarcodePartialNG OUTPUT;
				EXEC dbo.GenerateNumerator @Prefix = @prefixPallet, @LengthSequence = 4, @Result = @NewRefNo OUTPUT;	
			
				insert into ReceiptSupplyHistory 
				(
					[Status], ProcessMenu, RefNo, 
					WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
					RefWarehouseCode, RefAreaCode, RefAddressCode, RefBarcodeNo, RefNo2,
					QtyTrans, Remarks, ReferenceNo, LogDate, UserID
				)
				values
				(
					'OUT', 'Material NG', @refNo, 
					@warehouse, @area, @address, @ItemCode, @BarcodeNo, @lotNo, 
					@warehouse, 'TMP', 'TMP', @NewBarcodePartialNG, @NewRefNo,
					@QtyNG, 'Material NG Process', cast(@InspectionId as varchar), getdate(), @UserId
				)

				declare @QtySisa numeric(18,9) = @Qty - @QtyNG
				EXEC sp_Wms_Stock_UpSertStockDetail @refNo, @warehouse, @area, @address, @ItemCode, @BarcodeNo, @lotNo, @QtySisa, @InventoryQty, NULL, @UserId, 'OK', ''
				EXEC sp_Wms_Stock_UpSertStockHeader @transDate, @refNo, @warehouse, @area, @ItemCode, @lotNo, @QtyNG, NULL, 'S', @UserId

				EXEC sp_Wms_Stock_UpSertStockDetail @NewRefNo, @warehouse, 'TMP', 'TMP', @ItemCode, @NewBarcodePartialNG, @lotNo, @QtyNG, NULL, NULL, @UserId, 'HOLD', 'Process'
				EXEC sp_Wms_Stock_UpSertStockHeader @transDate, @NewRefNo, @warehouse, 'TMP', @ItemCode, @lotNo, @QtyNG, NULL, 'R', @UserId

				insert into Barcode_Split(Warehouse_Code, Area_Code, Address_Code, BarcodeNo, BarcodeNo_Original, Item_Code, Lot_No, Qty, Print_Cls, Supplier, RegisterDate, RegisterUser)
				values (@warehouse, 'TMP', 'TMP', @NewBarcodePartialNG, @BarcodeNo, @ItemCode, @lotNo, @QtyNG, 0, null, getdate(), @UserId)

				insert into BarcodeNGDetail (ReceiptId, DNNumber, SupplierCode, ItemCode, PONumber, BarcodeOriginal, BarcodeNew, QtyNG)
				values (@ReceiptId, @DNNumber, @SupplierCode, @ItemCode, null, @BarcodeNo, @NewBarcodePartialNG, @QtyNG)

				insert into ReceiptSupplyHistory 
				(
					[Status], ProcessMenu, RefNo, 
					WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
					RefWarehouseCode, RefAreaCode, RefAddressCode, RefBarcodeNo, RefNo2,
					QtyTrans, Remarks, ReferenceNo, LogDate, UserID
				)
				select 
					'IN', 'Material NG', RefNo, 
					sd.WarehouseCode, sd.AreaCode, sd.AddressCode, sd.ItemCode, sd.BarcodeNo, sd.LotNo, 
					@warehouse, @area, @address, @BarcodeNo, @refNo,
					sd.Qty, 'Material NG Process', cast(@InspectionId as varchar), getdate(), @UserId
				FROM StockDetail sd
				where sd.BarcodeNo = @NewBarcodePartialNG and sd.Qty > 0
			end
			else if @Qty = @QtyNG
			begin
				insert into ReceiptSupplyHistory 
				(
					[Status], ProcessMenu, RefNo, 
					WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
					QtyTrans, Remarks, ReferenceNo, LogDate, UserID
				)
				values
				(
					'OUT', 'Material NG', @refNo, 
					@warehouse, @area, @address, @ItemCode, @BarcodeNo, @lotNo, 
					@QtyNG, 'Material NG Process', cast(@InspectionId as varchar), getdate(), @UserId
				)

				EXEC dbo.GenerateNumerator @Prefix = @prefixPallet, @LengthSequence = 4, @Result = @NewRefNo OUTPUT;	
				
				EXEC sp_Wms_Stock_UpSertStockDetail @refNo, @warehouse, @area, @address, @ItemCode, @BarcodeNo, @lotNo, 0, NULL, NULL, @UserId, 'HOLD', 'Process'
				EXEC sp_Wms_Stock_UpSertStockHeader @transDate, @refNo, @warehouse, @area, @ItemCode, @lotNo, @Qty, NULL, 'S', @UserId

				EXEC sp_Wms_Stock_UpSertStockDetail @NewRefNo, @warehouse, 'TMP', 'TMP', @ItemCode, @NewBarcodePartialNG, @lotNo, @Qty, NULL, NULL, @UserId, 'HOLD', 'Process'
				EXEC sp_Wms_Stock_UpSertStockHeader @transDate, @NewRefNo, @warehouse, 'TMP', @ItemCode, @lotNo, @Qty, NULL, 'R', @UserId

				insert into ReceiptSupplyHistory 
				(
					[Status], ProcessMenu, RefNo, 
					WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
					QtyTrans, Remarks, ReferenceNo, LogDate, UserID
				)
				values
				(
					'IN', 'Material NG', @NewRefNo, 
					@warehouse, @area, @address, @ItemCode, @BarcodeNo, @lotNo, 
					@QtyNG, 'Material NG Process', cast(@InspectionId as varchar), getdate(), @UserId
				)
			end

			insert into IQC_SamplingBarcodeDetail 
			(
				InspectionID
				, BarcodeNo
				, BarcodeNG
				, CurrentStock
				, SampleQTY
				, RegisterDate
				, RegisterUser
			)
			VALUES
			(
				@InspectionId,
				@BarcodeNo,
				case when @QtyNG < @Qty then @NewBarcodePartialNG when @QtyNG = @Qty then @BarcodeNo else null end,
				@Qty,
				@QtyNG,
				getdate(),
				@UserId
			)

		end
		else
		begin
			update IQC_SamplingBarcodeDetail set SampleQTY = @QtyNG where InspectionID = @InspectionId and BarcodeNo = @BarcodeNo

			declare @barcodeNG varchar(50) = (select BarcodeNG From IQC_SamplingBarcodeDetail where InspectionID = @InspectionId and BarcodeNo = @BarcodeNo)
			if isnull(@barcodeNG, '') <> ''
			begin
				declare @refNoNG varchar(50), @lotNoNG varchar(100), 
						@warehouseNG varchar(25), @areaNG varchar(25), @addressNG varchar(25), @qtyBarcodeNG numeric(18,9), @ivtQtyBarcodeNG numeric(18,9)
				select 
					@refNoNG = RefNo, @lotNoNG = LotNo, @warehouseNG = WarehouseCode, @areaNG = AreaCode, @addressNG = AddressCode, 
					@qtyBarcodeNG = Qty, @ivtQtyBarcodeNG = InventoryQty
				From StockDetail where BarcodeNo = @barcodeNG and Qty > 0

				if @QtyNG <> @qtyBarcodeNG
				begin
					declare @supplyQty numeric(18,9) = (@qtyBarcodeNG * -1)

					insert into ReceiptSupplyHistory 
					(
						[Status], ProcessMenu, RefNo, 
						WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
						QtyTrans, Remarks, ReferenceNo, LogDate, UserID
					)
					values
					(
						'IN', 'Material NG', @refNoNG, 
						@warehouseNG, @areaNG, @addressNG, @ItemCode, @barcodeNG, @lotNoNG, 
						@supplyQty, 'Material NG Process', cast(@InspectionId as varchar), getdate(), @UserId
					),
					(
						'OUT', 'Material NG', @refNoNG, 
						@warehouseNG, @areaNG, @addressNG, @ItemCode, @BarcodeNo, @lotNoNG, 
						@supplyQty, 'Material NG Process', cast(@InspectionId as varchar), getdate(), @UserId
					)

					EXEC sp_Wms_Stock_UpSertStockDetail @refNoNG, @warehouseNG, @areaNG, @addressNG, @ItemCode, @barcodeNG, @lotNoNG, @QtyNG, @ivtQtyBarcodeNG, NULL, @UserId, 'HOLD', 'Process'
					EXEC sp_Wms_Stock_UpSertStockHeader @transDate, @refNo, @warehouse, @area, @ItemCode, @lotNo, @supplyQty, NULL, 'S', @UserId
					EXEC sp_Wms_Stock_UpSertStockHeader @transDate, @refNo, @warehouse, @area, @ItemCode, @lotNo, @QtyNG, NULL, 'R', @UserId

					insert into ReceiptSupplyHistory 
					(
						[Status], ProcessMenu, RefNo, 
						WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
						QtyTrans, Remarks, ReferenceNo, LogDate, UserID
					)
					values
					(
						'IN', 'Material NG', @refNoNG, 
						@warehouseNG, @areaNG, @addressNG, @ItemCode, @barcodeNG, @lotNoNG, 
						@qtyBarcodeNG, 'Material NG Process', cast(@InspectionId as varchar), getdate(), @UserId
					),
					(
						'OUT', 'Material NG', @refNoNG, 
						@warehouseNG, @areaNG, @addressNG, @ItemCode, @BarcodeNo, @lotNoNG, 
						@qtyBarcodeNG, 'Material NG Process', cast(@InspectionId as varchar), getdate(), @UserId
					)
				end
			end
		end
				
		select @InspectionId

		commit transaction ngTransaction
	end try
	begin catch
		rollback transaction ngTransaction
		declare @msg varchar(max) = (select ERROR_MESSAGE())
		raiserror(@msg, 16, 1)
		return
	end catch

end
