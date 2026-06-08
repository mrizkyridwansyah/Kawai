



CREATE   procedure [dbo].[sp_Wms_Mobile_SupplySubcon_Submit]
    @BarcodeNo VARCHAR(100),
    @LotNo VARCHAR(100),
    @ItemCode VARCHAR(100),
    @RequestNoCode VARCHAR(100),
    @ClassificationCode VARCHAR(25),
    @Qty NUMERIC(18,5),
    @UserID VARCHAR(100)
AS
BEGIN
    DECLARE @MsgErr VARCHAR(MAX)

	declare @validSO varchar(max) = (select dbo.fn_ValidateTransactionPeriod())

	if @validSO <> 'OK'
	begin
        RAISERROR(@validSO,16,1)
        RETURN
	end
	
	DECLARE @TablePallet TABLE(PalletNo VARCHAR(100))	
    DECLARE 
		@ToPalletNo VARCHAR(100),
        @WarehouseFrom VARCHAR(MAX), 
        @Location_Code VARCHAR(MAX),
        @Expired_Date VARCHAR(MAX), 
        @ToWareHouseCode VARCHAR(MAX),
        @ToLocationCode VARCHAR(25),
        @ParentItemCode VARCHAR(30),
        @InstructionNo VARCHAR(100),
        @ProductionID NUMERIC(18,0),
        @Date DATE = GETDATE()

    DECLARE @ReqDetailId BIGINT, @RequestId bigint, @WarehouseSubcon varchar(25)
	SELECT 
		@ReqDetailId = dtl.RequestDetailID, @RequestId = dtl.RequestID, @WarehouseSubcon = sp.Subcon_WH_Code
	FROM PartMaterialRequestDetail_PO dtl
	inner join PartMaterialRequestHeader_PO hd on dtl.RequestID = hd.RequestID
	inner join PurchaseOrder_Master po on hd.PO_NO = po.PO_No
	inner join Trade_Master sp on po.Supplier_Code = sp.Trade_Code
	WHERE RefNumber = @RequestNoCode and AreaCode = @ClassificationCode

	if isnull(@WarehouseSubcon, '') = ''
	begin
        SET @MsgErr = 'Data Supplier belum memiliki Warehouse Subcon!' 
        RAISERROR(@MsgErr,16,1)
        RETURN
	end

    DECLARE @IDSeq BIGINT, @QtySisa numeric(18,9)
	select 
		@IDSeq = pid.IDSeq, @QtySisa = (ChildRequirement_Qty - isnull(scan.TotalScan, 0))
	from PartMaterialRequestItemDetail_PO pid
	left join 
	(
		select IDSeq, sum(Qty) TotalScan from PartMaterialRequestItemDetailScan_PO
		group by IDSeq
	) scan on pid.IDSeq = scan.IDSeq
	where RequestDetailID = @ReqDetailId and ItemCode = @ItemCode

    IF isnull(@IDSeq, 0) = 0
    BEGIN
        SET @MsgErr = 'Item tidak terdaftar dalam list material request !' 
        RAISERROR(@MsgErr,16,1)
        RETURN
    END

    IF @Qty > @QtySisa
    BEGIN
        SET @MsgErr = 'Qty Scan tidak boleh melebihi dari Qty Plan' 
        RAISERROR(@MsgErr,16,1)
        RETURN
    END

    IF NOT EXISTS (SELECT 1 FROM StockDetail WHERE BarcodeNo = @BarcodeNo and Qty > 0)
    BEGIN
        SET @MsgErr = 'Material tidak ditemukan / sudah habis' 
        RAISERROR(@MsgErr,16,1)
        RETURN
    END

	DECLARE
        @FromRefNo VARCHAR(MAX),
        @FromWarehouseCode VARCHAR(MAX), 
        @FromAreaCode VARCHAR(MAX),
        @FromAddressCode VARCHAR(MAX),
        @QtyStock NUMERIC(18,9),
		@PickingNo varchar(100),
		@StatusReceipt varchar(100)

    SELECT 
        @FromRefNo = RefNo, 
        @FromWarehouseCode = WarehouseCode, 
        @FromAreaCode = AreaCode, 
        @FromAddressCode = AddressCode,
        @QtyStock = ISNULL(Qty,0),
		@PickingNo = Picking_No,
		@StatusReceipt = StatusReceipt
    FROM dbo.StockDetail 
    WHERE BarcodeNo = @BarcodeNo AND ISNULL(Qty,0) > 0

	if @FromWarehouseCode in (select Subcon_WH_Code from Trade_Master where Trade_Cls = '3') 
	begin
		raiserror('Lokasi stock berada di warehouse subcon!', 16,1)
		return
	end

	if @StatusReceipt <> 'OK'
	begin
        RAISERROR('Status Barcode belum OK',16,1)
        RETURN
	end

	if isnull(@PickingNo, '') <> ''
	begin
        RAISERROR('Barcode sudah disupply',16,1)
        RETURN
	end

    IF @Qty > @QtyStock
    BEGIN
        RAISERROR('Qty tidak bisa melebihi Qty Stock',16,1)
        RETURN
    END

	declare @RequestStatusID int
    SELECT @ToPalletNo = RackNumber, @RequestStatusID = RequestStatusID
	FROM PartMaterialRequestDetail_PO
    WHERE RequestDetailID = @ReqDetailId AND AreaCode = @ClassificationCode

	if @RequestStatusID = 5
	BEGIN
		raiserror('Status Request No already complete!', 16,1)
		return
	end

    DECLARE @NewBarcode VARCHAR(50)

	IF @QtyStock > @Qty
	begin
		declare @factoryCode varchar(25) = (select Company_Code From WareHouse_Master where WH_Code = @FromWarehouseCode)
		declare @prefixFactory varchar(5) = (select PrefixGlobalBarcode From Company_Profile where Company_Code = @factoryCode)

		declare @dt varchar(8) = format(getdate(), 'yyyyMMdd')
		declare @prefixBarcode varchar(20) = @prefixFactory + 'FQR' + @dt
		EXEC dbo.GenerateNumerator @Prefix = @prefixBarcode, @LengthSequence = 4, @Result = @NewBarcode OUTPUT;			
	end

    IF ISNULL(@ToPalletNo,'') = ''
    BEGIN
        INSERT INTO @TablePallet
        EXEC sp_Wms_Stock_GeneratePalletNo
        SELECT @ToPalletNo = PalletNo FROM @TablePallet
    END

	BEGIN TRY
		BEGIN TRANSACTION SupplySubconTrans;

        update PartMaterialRequestDetail_PO set RackNumber = @ToPalletNo where RequestDetailID = @ReqDetailId AND AreaCode = @ClassificationCode

		IF @QtyStock > @Qty
		BEGIN
			PRINT('split')
			IF NOT EXISTS(
				SELECT 1 
				FROM PartMaterialRequestItemDetailScan_PO 
				WHERE IDSeq = @IDSeq AND BarcodeNo = @NewBarcode AND ItemCode = @ItemCode AND LotNo = @LotNo
			)
			BEGIN
				DECLARE @QtyOutstanding NUMERIC(18,9) = (@QtyStock - @Qty)

				INSERT INTO PartMaterialRequestItemDetailScan_PO
				(
					IDSeq, FromRefNo, FromWarehouseCode, FromAreaCode, FromAddressCode,
					ToRefNo, ToWarehouseCode, ToAreaCode, ToAddressCode, BarcodeNo,
					ItemCode, LotNo, Qty, BarcodeNoOriginal, RegisterDate, RegisterUser
				)
				VALUES
				(
					@IDSeq, @FromRefNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode,
					@ToPalletNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode, @NewBarcode,
					@ItemCode, @LotNo, @Qty, @BarcodeNo, GETDATE(), @UserID
				)
			END

			-- Update stock & insert history
			EXEC sp_Wms_Stock_UpSertStockDetail @FromRefNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode, @ItemCode, @BarcodeNo, @LotNo, @QtyOutstanding, NULL, 0, @UserID, 'OK'
			EXEC sp_Wms_Stock_UpSertStockHeader @Date, @FromRefNo, @FromWarehouseCode, @FromAreaCode, @ItemCode, @LotNo, @Qty, NULL, 'S', @UserID

			INSERT INTO ReceiptSupplyHistory 
			(
				[Status], ProcessMenu, RefNo, WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
				RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo,
				QtyTrans, Remarks, ReferenceNo, LogDate, UserID
			)
			SELECT 
				'OUT', 'Mobile Supply Scan Request', @FromRefNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode, @ItemCode, @BarcodeNo, @LotNo, 
				@WarehouseSubcon, 'TMP', 'TMP', @ItemCode, @BarcodeNo, @LotNo, 
				@Qty, 'Mobile Supply Subcon ke palet ' + ISNULL(@ToPalletNo,''), @ToPalletNo,
				GETDATE(), @UserID

			exec sp_Wms_Stock_UpSertStockDetail @ToPalletNo, @WarehouseSubcon, 'TMP', 'TMP', @ItemCode, @NewBarcode, @LotNo, @Qty, NULL, 0, @UserId, 'OK', ''
			exec sp_Wms_Stock_UpSertStockHeader @Date, @ToPalletNo, @WarehouseSubcon, 'TMP', @ItemCode, @LotNo, @Qty, NULL, 'R', @UserId

			Update StockDetail Set Picking_No = @RequestNoCode where BarcodeNo = @NewBarcode and qty> 0

			insert into ReceiptSupplyHistory 
			(
				[Status], ProcessMenu, RefNo, WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
				RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
				QtyTrans, Remarks, ReferenceNo, LogDate, UserID
			)
			select 
				'IN', 'Mobile Supply Scan Request', @ToPalletNo, @WarehouseSubcon, 'TMP', 'TMP', @ItemCode, @NewBarcode, @LotNo, 
				@FromWarehouseCode, @FromAreaCode, @FromAddressCode, @ItemCode, @BarcodeNo, @LotNo, 
				@Qty, 'Mobile Supply Scan Request dari palet ' + isnull(@FromRefNo, ''), @FromRefNo, 
				getdate(), @UserId

			INSERT INTO dbo.Barcode_Split 
			( 
				Warehouse_Code, Area_Code, Address_Code, BarcodeNo, Item_Code, Lot_No, SublotNo, Qty, InventoryQty,
				Print_Cls, Expired_Date, Production_Date, Receipt_Date, Supplier, BarcodeNo_Original, RegisterUser, RegisterDate, Last_update, Last_User
			)
			SELECT 
				WarehouseCode, AreaCode, AddressCode, BarcodeNo, ItemCode, LotNo, SublotNo, Qty, InventoryQty,
				NULL, ExpiredDate, ProductionDate, ReceiptDate, Supplier, @BarcodeNo, @UserID, GETDATE(), NULL, NULL
			FROM dbo.StockDetail
			WHERE BarcodeNo = @NewBarcode AND ISNULL(Qty,0)>0        
		END
		ELSE
		BEGIN
			PRINT('Full Qty')

			IF NOT EXISTS(
				SELECT 1 
				FROM PartMaterialRequestItemDetailScan_PO 
				WHERE IDSeq = @IDSeq AND BarcodeNo = @BarcodeNo AND ItemCode = @ItemCode AND LotNo = @LotNo
			)
			BEGIN
				INSERT INTO PartMaterialRequestItemDetailScan_PO
				(
					IDSeq, FromRefNo, FromWarehouseCode, FromAreaCode, FromAddressCode,
					ToRefNo, ToWarehouseCode, ToAreaCode, ToAddressCode, BarcodeNo,
					ItemCode, LotNo, Qty, BarcodeNoOriginal, RegisterDate, RegisterUser
				)
				VALUES
				(
					@IDSeq, @FromRefNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode,
					@ToPalletNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode, @BarcodeNo,
					@ItemCode, @LotNo, @Qty, @BarcodeNo, GETDATE(), @UserID
				)
			END

			-- Update stock & insert history
			EXEC sp_Wms_Stock_UpSertStockDetail @FromRefNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode, @ItemCode, @BarcodeNo, @LotNo, 0, NULL, 0, @UserID, 'OK', ''
			EXEC sp_Wms_Stock_UpSertStockHeader @Date, @FromRefNo, @FromWarehouseCode, @FromAreaCode, @ItemCode, @LotNo, @Qty, NULL, 'S', @UserID

			INSERT INTO ReceiptSupplyHistory 
			(
				[Status], ProcessMenu, RefNo, WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
				RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo,
				QtyTrans, Remarks, ReferenceNo, LogDate, UserID
			)
			SELECT 
				'OUT', 'Mobile Supply Scan Request', @FromRefNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode, @ItemCode, @BarcodeNo, @LotNo, 
				@WarehouseSubcon, 'TMP', 'TMP', @ItemCode, @BarcodeNo, @LotNo, 
				@Qty, 'Mobile Supply Subcon ke palet ' + ISNULL(@ToPalletNo,''), @ToPalletNo, GETDATE(), @UserID

			exec sp_Wms_Stock_UpSertStockDetail @ToPalletNo, @WarehouseSubcon, 'TMP', 'TMP', @ItemCode, @BarcodeNo, @LotNo, @Qty, NULL, 0, @UserId, 'OK', ''
			exec sp_Wms_Stock_UpSertStockHeader @Date, @ToPalletNo, @WarehouseSubcon, 'TMP', @ItemCode, @LotNo, @Qty, NULL, 'R', @UserId

			Update StockDetail Set Picking_No = @RequestNoCode where BarcodeNo = @BarcodeNo and qty> 0

			insert into ReceiptSupplyHistory 
			(
				[Status], ProcessMenu, RefNo, WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
				RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
				QtyTrans, Remarks, ReferenceNo, LogDate, UserID
			)
			select 
				'IN', 'Mobile Supply Scan Request', @ToPalletNo, @WarehouseSubcon, 'TMP', 'TMP', @ItemCode, @BarcodeNo, @LotNo, 
				@FromWarehouseCode, @FromAreaCode, @FromAddressCode, @ItemCode, @BarcodeNo, @LotNo, 
				@Qty, 'Mobile Supply Subcon dari palet ' + isnull(@FromRefNo, ''), @FromRefNo, getdate(), @UserId
		END

		IF NOT EXISTS 
		(
			Select 1 from 
			(
				select A.ItemCode, RequestDetailID, ChildRequirement_Qty - ISNULL(SUM(B.Qty),0) Nilai 
				from PartMaterialRequestItemDetail_PO A 
				LEFT JOIN PartMaterialRequestItemDetailScan_PO B ON A.IDSeq = B.IDSeq and A.ItemCode = B.ItemCode  
				where A.RequestDetailID = @ReqDetailId
				Group by RequestDetailID, ChildRequirement_Qty , A.ItemCode
			) A where Nilai > 0
		)
		BEGIN
			Update PartMaterialRequestDetail_PO Set RequestStatusID = 5 where RefNumber = @RequestNoCode
		END
	
		COMMIT TRANSACTION SupplySubconTrans;
	END TRY
	BEGIN CATCH
		SET @msgErr = ERROR_MESSAGE()
		ROLLBACK TRANSACTION SupplySubconTrans;
		raiserror(@msgErr, 16, 1)
		RETURN;
	END CATCH

END
