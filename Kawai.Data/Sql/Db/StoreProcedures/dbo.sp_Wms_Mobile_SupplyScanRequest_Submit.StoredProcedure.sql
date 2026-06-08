



CREATE   procedure [dbo].[sp_Wms_Mobile_SupplyScanRequest_Submit]
    @WarehouseCode VARCHAR(100),
    @BarcodeNo VARCHAR(100),
    @LineCode VARCHAR(100),
    @LotNo VARCHAR(100),
    @ItemCode VARCHAR(100),
    @RequestNoCode VARCHAR(100),
    @ItemClass VARCHAR(25),
    @Qty NUMERIC(18,5),
    @UserID VARCHAR(100)
AS
BEGIN
    DECLARE @TablePallet TABLE(PalletNo VARCHAR(100))	
    DECLARE 
		@ToPalletNo VARCHAR(100),
        @FromRefNo VARCHAR(MAX),
        @FromWarehouseCode VARCHAR(MAX), 
        @FromAreaCode VARCHAR(MAX),
        @FromAddressCode VARCHAR(MAX),
        @WarehouseFrom VARCHAR(MAX), 
        @Location_Code VARCHAR(MAX),
        @Expired_Date VARCHAR(MAX), 
        @ToWareHouseCode VARCHAR(MAX),
        @ToLocationCode VARCHAR(25),
        @LastQty NUMERIC(18,9),
        @ParentItemCode VARCHAR(30),
        @InstructionNo VARCHAR(100),
        @ProductionID NUMERIC(18,0),
		@PickingNo varchar(100),
		@StatusReceipt varchar(100),
        @Date DATE = GETDATE()

    DECLARE @ReqID BIGINT
    SELECT @ReqID = RequestDetailID 
    FROM PartMaterialRequestDetail 
    WHERE RefNumber = @RequestNoCode and AreaCode = @ItemClass

    SELECT 
        b.ItemCode, 
        c.Item_Name AS ItemName, 
        b.ChildRequirement_Qty AS PlanQty, 
        ISNULL((SELECT SUM(Qty) 
                FROM PartMaterialRequestItemDetailScan cc 
                WHERE cc.ItemCode = b.ItemCode AND cc.IDSeq = B.IDSeq),0) AS QtyScan, 
        b.Unit_Cls, 
        d.[Description] AS UnitDesc, 
        b.RequestDetailID,
        @RequestNoCode AS RequestNo,
        B.IDSeq  
    INTO #zTempData
    FROM PartMaterialRequestDetail A 
    LEFT JOIN PartMaterialRequestItemDetail B ON A.RequestDetailID = B.RequestDetailID
    LEFT JOIN Item_Master c ON b.ItemCode = c.Item_Code
    LEFT JOIN Unit_Cls d ON d.Unit_Cls = b.Unit_Cls
    WHERE A.RequestDetailID = @ReqID

    DECLARE @Msg VARCHAR(MAX) = ''
    DECLARE @IDSeq INT
    SELECT @IDSeq = IDSeq 
    FROM #zTempData 
    WHERE ItemCode = @ItemCode

    DECLARE @MsgErr VARCHAR(MAX)
    DECLARE @IvtYear INT, @IvtMonth INT, @StartPeriod DATETIME, @EndPeriod DATETIME

	--declare @validSO varchar(max) = (select dbo.fn_ValidateTransactionPeriod())

	--if @validSO <> 'OK'
	--begin
 --       RAISERROR(@validSO,16,1)
 --       RETURN
	--end

    IF NOT EXISTS (SELECT TOP 1 1 FROM #zTempData WHERE ItemCode = @ItemCode)
    BEGIN
        SET @Msg = 'Item tidak terdaftar dalam list material request !' 
        RAISERROR(@Msg,16,1)
        RETURN
    END

    IF NOT EXISTS 
	(
		SELECT RefNo, WarehouseCode, AreaCode, AddressCode, BarcodeNo, ItemCode, LotNo, Qty 
        FROM StockDetail
        WHERE BarcodeNo = @BarcodeNo and Qty > 0
	)
    BEGIN
        SET @Msg = 'Material tidak ditemukan / sudah habis' 
        RAISERROR(@Msg,16,1)
        RETURN
    END

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

    DECLARE @QtyScan NUMERIC(18,0)
    SELECT @QtyScan = ISNULL(PlanQty,0) - ISNULL(QtyScan,0) 
    FROM #zTempData 
    WHERE ItemCode = @ItemCode

    IF @Qty > @QtyScan
    BEGIN
        SET @Msg = 'Qty Scan tidak boleh melebihi dari Qty Plan' 
        RAISERROR(@Msg,16,1)
        RETURN
    END

    SELECT 
        @FromRefNo = RefNo, 
        @FromWarehouseCode = WarehouseCode, 
        @FromAreaCode = AreaCode, 
        @FromAddressCode = AddressCode,
		@PickingNo = Picking_No,
		@StatusReceipt = StatusReceipt,
        @LastQty = ISNULL(Qty,0)
    FROM dbo.StockDetail with (updlock, rowlock)
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

    IF @Qty > @LastQty
    BEGIN
        RAISERROR('Qty tidak bisa melebihi Qty Stock',16,1)
        RETURN
    END

    DECLARE @StopPoint VARCHAR(MAX)
    SELECT @StopPoint = StopPointCode FROM MS_Address WHERE AddressCode = @FromAddressCode

    IF @StopPoint IS NULL
    BEGIN
        RAISERROR('Setting Stop poin terlebih dahulu!',16,1)
        RETURN
    END

    DECLARE @NewBarcode VARCHAR(50)

	IF @LastQty > @Qty
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
	end

    BEGIN TRY  
		BEGIN TRANSACTION SupplyTransaction

        SELECT @ToPalletNo = RefNo 
        FROM PartMaterialRequestDetailPallet 
        WHERE RequestDetailID = @ReqID AND Stop_Point = @StopPoint

        IF ISNULL(@ToPalletNo,'') = ''
        BEGIN
            INSERT INTO PartMaterialRequestDetailPallet 
            VALUES(@ReqID, @ToPalletNo, GETDATE(), @UserID, @StopPoint)
        END

        IF @LastQty > @Qty
        BEGIN
            PRINT('split')

            IF NOT EXISTS(
                SELECT 1 
                FROM PartMaterialRequestItemDetailScan 
                WHERE IDSeq = @IDSeq AND BarcodeNo = @NewBarcode AND ItemCode = @ItemCode AND LotNo = @LotNo
            )
            BEGIN
                DECLARE @QtyOriginal NUMERIC(18,5), @QtyOutstanding NUMERIC(18,5)
                SELECT @QtyOriginal = ISNULL(Qty,0) 
                FROM StockDetail 
                WHERE BarcodeNo = @BarcodeNo AND Qty > 0

                SET @QtyOutstanding = @QtyOriginal - @Qty

                INSERT INTO PartMaterialRequestItemDetailScan
                (
                    IDSeq, FromRefNo, FromWarehouseCode, FromAreaCode, FromAddressCode,
                    ToRefNo, ToWarehouseCode, ToAreaCode, ToAddressCode, BarcodeNo,
                    ItemCode, LotNo, Qty, BarcodeNo_Original, RegisterDate, RegisterUser
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
                'OUT', 'Mobile Supply Scan Request', @FromRefNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode,
                @ItemCode, @BarcodeNo, @LotNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode,
                @ItemCode, @BarcodeNo, @LotNo, @Qty, 'Mobile Supply Scan Request ke palet ' + ISNULL(@ToPalletNo,''), @ToPalletNo,
                GETDATE(), @UserID

			exec sp_Wms_Stock_UpSertStockDetail @ToPalletNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode, @ItemCode, @NewBarcode, @LotNo, @Qty, NULL, 0, @UserId, 'OK'
			exec sp_Wms_Stock_UpSertStockHeader @Date, @ToPalletNo, @FromWarehouseCode, @FromAreaCode, @ItemCode, @LotNo, @Qty, NULL, 'R', @UserId

			Update StockDetail Set Picking_No = @RequestNoCode where BarcodeNo = @NewBarcode and qty> 0

			insert into ReceiptSupplyHistory 
			(
				[Status], ProcessMenu, RefNo, WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
				RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
				QtyTrans, Remarks, ReferenceNo, LogDate, UserID
			)
			select 
				'IN', 'Mobile Supply Scan Request', @ToPalletNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode, 
				@ItemCode, @NewBarcode, @LotNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode, 
				@ItemCode, @NewBarcode, @LotNo, @QtyOutstanding, 'Mobile Supply Scan Request dari palet ' + isnull(@FromRefNo, ''), @FromRefNo, 
				getdate(), @UserId


			INSERT INTO dbo.Barcode_Split 
			( 
				Warehouse_Code, Area_Code, Address_Code, BarcodeNo, Item_Code, Lot_No, SublotNo, Qty, InventoryQty,
				Print_Cls, Expired_Date, Production_Date, Receipt_Date, Supplier, BarcodeNo_Original, RegisterUser, RegisterDate, Last_update, Last_User
			)
			SELECT 
				@FromWarehouseCode, @FromAreaCode, @FromAddressCode, @NewBarcode, @ItemCode, @LotNo, SublotNo, ISNULL(@Qty,0), InventoryQty,
				NULL, ExpiredDate, ProductionDate, ReceiptDate, Supplier, @BarcodeNo, @UserID, GETDATE(), NULL, NULL
			FROM dbo.StockDetail A
			WHERE RefNo = @FromRefNo 
			and WarehouseCode=@FromWarehouseCode 
			AND AreaCode=@FromAreaCode 
			and AddressCode = @FromAddressCode 
			AND BarcodeNo=@BarcodeNo 
			AND ItemCode=@ItemCode 
			AND LotNo=@LotNo 
			AND ISNULL(A.Qty,0)>0        
		END
        ELSE
        BEGIN
            PRINT('Full Qty')

            IF NOT EXISTS(
                SELECT 1 
                FROM PartMaterialRequestItemDetailScan 
                WHERE IDSeq = @IDSeq AND BarcodeNo = @BarcodeNo AND ItemCode = @ItemCode AND LotNo = @LotNo
            )
            BEGIN
				INSERT INTO PartMaterialRequestItemDetailScan
                (
                    IDSeq, FromRefNo, FromWarehouseCode, FromAreaCode, FromAddressCode,
                    ToRefNo, ToWarehouseCode, ToAreaCode, ToAddressCode, BarcodeNo,
                    ItemCode, LotNo, Qty, BarcodeNo_Original, RegisterDate, RegisterUser
                )
                VALUES
                (
                    @IDSeq, @FromRefNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode,
                    @ToPalletNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode, @BarcodeNo,
                    @ItemCode, @LotNo, @Qty, @BarcodeNo, GETDATE(), @UserID
                )
            END

            -- Update stock & insert history
            EXEC sp_Wms_Stock_UpSertStockDetail @FromRefNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode, @ItemCode, @BarcodeNo, @LotNo, 0, NULL, 0, @UserID, 'OK'
            EXEC sp_Wms_Stock_UpSertStockHeader @Date, @FromRefNo, @FromWarehouseCode, @FromAreaCode, @ItemCode, @LotNo, @Qty, NULL, 'S', @UserID

            INSERT INTO ReceiptSupplyHistory 
            (
                [Status], ProcessMenu, RefNo, WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
                RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo,
                QtyTrans, Remarks, ReferenceNo, LogDate, UserID
            )
            SELECT 
                'OUT', 'Mobile Supply Scan Request', @FromRefNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode,
                @ItemCode, @BarcodeNo, @LotNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode,
                @ItemCode, @BarcodeNo, @LotNo, @Qty, 'Mobile Supply Scan Request ke palet ' + ISNULL(@ToPalletNo,''), @ToPalletNo,
                GETDATE(), @UserID

			exec sp_Wms_Stock_UpSertStockDetail @ToPalletNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode, @ItemCode, @BarcodeNo, @LotNo, @Qty, NULL, 0, @UserId, 'OK'
			exec sp_Wms_Stock_UpSertStockHeader @Date, @ToPalletNo, @FromWarehouseCode, @FromAreaCode, @ItemCode, @LotNo, @Qty, NULL, 'R', @UserId

			Update StockDetail Set Picking_No = @RequestNoCode where BarcodeNo = @BarcodeNo and qty> 0

			insert into ReceiptSupplyHistory 
			(
				[Status], ProcessMenu, RefNo, WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
				RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
				QtyTrans, Remarks, ReferenceNo, LogDate, UserID
			)
			select 
				'IN', 'Mobile Supply Scan Request', @ToPalletNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode, 
				@ItemCode, @BarcodeNo, @LotNo, @FromWarehouseCode, @FromAreaCode, @FromAddressCode, 
				@ItemCode, @BarcodeNo, @LotNo, @Qty, 'Mobile Supply Scan Request dari palet ' + isnull(@FromRefNo, ''), @FromRefNo, 
				getdate(), @UserId
		END

		DECLARE @hasComplete bit = 0

		IF NOT EXISTS 
		(
			Select * from 
			(
			    select A.ItemCode, a.RequestDetailID, ChildRequirement_Qty - ISNULL(SUM(B.Qty),0) Nilai 
				from PartMaterialRequestItemDetail A 
				inner join PartMaterialRequestDetail dtl on a.RequestDetailID = dtl.RequestDetailID
				LEFT JOIN PartMaterialRequestItemDetailScan B ON A.IDSeq = B.IDSeq and A.ItemCode = B.ItemCode  
				WHERE dtl.RefNumber = @RequestNoCode
				Group by a.RequestDetailID, ChildRequirement_Qty , A.ItemCode
			) A where Nilai > 0
		)
		BEGIN
			Update PartMaterialRequestDetail 
			Set 
				RequestStatusID = 5, 
				StatusAMR = 'Requesting to AMR',
				LastUpdate = getdate(), LastUser = @UserID, 
				LastRequestDateAMR = getdate(), LastUserRequestAMR = @UserID
			where RefNumber = @RequestNoCode

			EXEC SP_Scheduler_TransferDataRobot2 @RequestNoCode, @UserId

			SET @hasComplete = 1
		END

		SELECT @hasComplete HasComplete

		COMMIT TRANSACTION SupplyTransaction
    END TRY  
    BEGIN CATCH  
		ROLLBACK TRANSACTION SupplyTransaction

        SET @Msg = ERROR_MESSAGE()
        RAISERROR(@Msg,16,1)
        RETURN
    END CATCH 

    DROP TABLE #zTempData
END
