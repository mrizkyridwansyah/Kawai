USE [EZRunnerV3_KawaiLive]
GO

/****** Object:  StoredProcedure [dbo].[sp_Wms_Mobile_PickingByScan_Submit]    Script Date: 5/19/2026 9:05:13 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_Wms_Mobile_PickingByScan_Submit]
(
    @InstructionNo  VARCHAR(50),
    @BarcodeNo      VARCHAR(100),
    @DeviceID       VARCHAR(50),
    @UserID         VARCHAR(50)
)
AS
BEGIN

    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @PickingNo     VARCHAR(50),
        @ItemCode      VARCHAR(50),
        @SerialNo      VARCHAR(100),
        @Warehouse     VARCHAR(25),
        @Area          VARCHAR(25),
        @Address       VARCHAR(25),
        @LotNo         VARCHAR(100),
        @Qty           NUMERIC(18,9),
        @SeqNo         INT

    -------------------------------------------------------
    -- GET STOCK DATA
    -------------------------------------------------------
    SELECT TOP 1
        @ItemCode  = a.ItemCode,
        @SerialNo  = b.Serial_No,
        @Warehouse = a.WarehouseCode,
        @Area      = a.AreaCode,
        @Address   = a.AddressCode,
        @LotNo     = a.LotNo,
        @Qty       = a.Qty
    FROM dbo.StockDetail a WITH (UPDLOCK, HOLDLOCK)
    OUTER APPLY (
        SELECT TOP 1 x.Serial_No--, x.PO_NO, x.PO_SeqNo 
        FROM dbo.ShippingInstruction_Detail x
        WHERE x.SI_No = @InstructionNo AND x.Item_Code = a.ItemCode
    ) b
    WHERE BarcodeNo = @BarcodeNo AND Qty > 0

    IF @ItemCode IS NULL
    BEGIN
        RAISERROR('Barcode tidak ditemukan!',16,1)
        RETURN
    END

    -------------------------------------------------------
    -- VALIDASI STOK
    -------------------------------------------------------
    IF NOT EXISTS
    (
        SELECT 1
        FROM StockDetail
        WHERE BarcodeNo = @BarcodeNo
        AND Qty > 0
        AND StatusReceipt = 'OK'
    )
    BEGIN
        RAISERROR('Stock barcode sudah tidak valid!',16,1)
        RETURN
    END

    -------------------------------------------------------
    -- VALIDASI MULTIPLE USER ACCESS
    -------------------------------------------------------
    IF EXISTS
    (
        SELECT 1
        FROM StockDetail
        WHERE BarcodeNo = @BarcodeNo
        AND ISNULL(Picking_No,'') <> ''
    )
    BEGIN
        RAISERROR('Barcode sedang digunakan picking lain!',16,1)
        RETURN
    END

    -------------------------------------------------------
    -- VALIDASI SHIPPING DETAIL
    -------------------------------------------------------
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.ShippingInstruction_Detail
        WHERE
            SI_No = @InstructionNo
            AND Item_Code = @ItemCode
            AND Serial_No = @SerialNo
    )
    BEGIN
        RAISERROR('Barcode tidak sesuai Shipping Instruction!',16,1)
        RETURN
    END

    -------------------------------------------------------
    -- VALIDASI SUDAH PICKING
    -------------------------------------------------------
    IF EXISTS (
        SELECT 1
        FROM dbo.ShippingInstruction_Detail a
        JOIN dbo.PickingScan_Detail b ON b.SI_No = a.SI_No AND b.Item_Code = a.Item_Code AND b.Serial_No = a.Serial_No AND b.Barcode_No = @BarcodeNo
        WHERE
            a.SI_No = @InstructionNo
            AND a.Item_Code = @ItemCode
            AND a.Serial_No = @SerialNo
    )
    BEGIN
        RAISERROR('Serial sudah dipicking!',16,1)
        RETURN
    END

    -------------------------------------------------------
    -- VALIDASI DOUBLE SCAN
    -------------------------------------------------------
    IF EXISTS (
        SELECT 1
        FROM dbo.PickingScan_Detail
        WHERE
            SI_No = @InstructionNo
            AND Barcode_No = @BarcodeNo
    )
    BEGIN
        RAISERROR('Barcode sudah pernah discan!',16,1)
        RETURN
    END

    BEGIN TRY
        DECLARE 
            @DateNow    DATETIME = GETDATE(),
            @PO_No      VARCHAR(20),
            @PO_SeqNo   INT;

        SELECT TOP 1
            @PO_No = x.PO_NO, 
            @PO_SeqNo = x.PO_SeqNo
        FROM dbo.ShippingInstruction_Detail x
        WHERE x.SI_No = @InstructionNo AND x.Item_Code = @ItemCode AND x.Serial_No = @SerialNo

        BEGIN TRAN

        /* PINDAH STOK */

        DECLARE @ToRefNo varchar(50) = (SELECT RefNo FROM ShippingInstructionPallet WHERE ShippingNo = @InstructionNo AND ItemCode = @ItemCode)
        IF @ToRefNo IS NULL
        BEGIN
            DECLARE @prefixPallet varchar(20) = 'PLT.' + FORMAT(@DateNow, 'yyyyMMdd') + '.'
            EXEC dbo.GenerateNumerator @Prefix = @prefixPallet, @LengthSequence = 4, @Result = @ToRefNo OUTPUT

			INSERT INTO ShippingInstructionPallet (ShippingNo, ItemCode, RefNo, RegisterDate, RegisterUser)
			VALUES (@InstructionNo, @ItemCode, @ToRefNo, @DateNow, @UserID)
        END

        DECLARE @fromRef varchar(25)
        SELECT
            @fromRef = RefNo
        FROM StockDetail
        WHERE BarcodeNo = @BarcodeNo AND Qty > 0

		insert into ReceiptSupplyHistory 
		(
			[Status], ProcessMenu, RefNo, 
			WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
			RefNo2, RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
			QtyTrans, Remarks, ReferenceNo, LogDate, UserID
		)
		select 
			'OUT', 'Picking Scan Mobile', @fromRef, 
			@warehouse, @area, @address, @ItemCode, @BarcodeNo, @LotNo, 
			@ToRefNo, @warehouse, @area, @address, @ItemCode, @BarcodeNo, @LotNo, 
			@qty, 'Picking Scan Mobile ke ' + isnull(@ToRefNo, ''), @InstructionNo, 
			@DateNow, @UserId

        DECLARE @transDate date = @DateNow

		EXEC sp_Wms_Stock_UpSertStockHeader @transDate, @fromRef, @warehouse, @area, @ItemCode, @LotNo, @qty, NULL, 'S', @UserId
		EXEC sp_Wms_Stock_UpSertStockHeader @transDate, @ToRefNo, @warehouse, @area, @ItemCode, @LotNo, @qty, NULL, 'R', @UserId

		EXEC sp_Wms_Stock_UpSertStockDetail @fromRef, @warehouse, @area, @address, @ItemCode, @BarcodeNo, @LotNo, 0, NULL, NULL, @UserId, 'OK', NULL
		EXEC sp_Wms_Stock_UpSertStockDetail @ToRefNo, @warehouse, @area, @address, @ItemCode, @BarcodeNo, @LotNo, @qty, NULL, NULL, @UserId, 'OK', NULL

		insert into ReceiptSupplyHistory 
		(
			[Status], ProcessMenu, RefNo, 
			WarehouseCode, AreaCode, AddressCode, ItemCode, BarcodeNo, LotNo,
			RefNo2, RefWarehouseCode, RefAreaCode, RefAddressCode, RefItemCode, RefBarcodeNo, RefLotNo, 
			QtyTrans, Remarks, ReferenceNo, LogDate, UserID
		)
		select 
			'IN', 'Picking Scan Mobile', @ToRefNo, 
			@warehouse, @area, @address, @ItemCode, @BarcodeNo, @LotNo, 
			@fromRef, @warehouse, @area, @address, @ItemCode, @BarcodeNo, @LotNo, 
			@qty, 'Picking Scan Mobile ke ' + isnull(@ToRefNo, ''), @InstructionNo, 
			@DateNow, @UserId



        -------------------------------------------------------
        -- GET / CREATE PICKING HEADER
        -------------------------------------------------------
        SELECT TOP 1
            @PickingNo = Picking_No
        FROM dbo.PickingScan_Header
        WHERE
            SI_No = @InstructionNo
            AND Picking_Status = 'OPEN'

        IF @PickingNo IS NULL
        BEGIN

            DECLARE @Prefix VARCHAR(20)
            SET @Prefix = 'PKG.' + FORMAT(@DateNow,'yyyyMMdd') + '.'

            EXEC dbo.GenerateNumerator
                @Prefix = @Prefix,
                @LengthSequence = 4,
                @Result = @PickingNo OUTPUT

            INSERT INTO dbo.PickingScan_Header (
                Picking_No, SI_No, WH_Code, Picking_Status, Start_Date, Register_Date, Register_By
            )
            VALUES (
                @PickingNo, @InstructionNo, @Warehouse, 'OPEN', @DateNow, @DateNow, @UserID
            )
        END

        -------------------------------------------------------
        -- GENERATE SEQNO
        -------------------------------------------------------
        SELECT
            @SeqNo = ISNULL(MAX(SeqNo),0) + 1
        FROM dbo.PickingScan_Detail WITH (UPDLOCK,HOLDLOCK)
        WHERE Picking_No = @PickingNo

        -------------------------------------------------------
        -- INSERT DETAIL SCAN
        -------------------------------------------------------
        INSERT INTO dbo.PickingScan_Detail (
            Picking_No, SeqNo, SI_No, PO_No, PO_SeqNo, Item_Code, Serial_No, Barcode_No, WarehouseCode, AreaCode, AddressCode, 
            Scan_Status, Scan_Date, Scan_By, Submit_Status, Device_ID
        )
        VALUES (
            @PickingNo, @SeqNo, @InstructionNo, @PO_No, @PO_SeqNo, @ItemCode, @SerialNo, @BarcodeNo, @Warehouse, @Area, @Address,
            'VALID', @DateNow, @UserID, 1, @DeviceID
        )

        ---------------------------------------------------------
        ---- UPDATE SHIPPING DETAIL
        ---------------------------------------------------------
        --UPDATE dbo.ShippingInstruction_Detail
        --SET
        --    IsPicking = '1',
        --    Picking_Date = @DateNow,
        --    Picking_By = @UserID,
        --    Update_Date = @DateNow,
        --    Update_By = @UserID
        --WHERE
        --    SI_No = @InstructionNo
        --    AND Item_Code = @ItemCode
        --    AND Serial_No = @SerialNo

        -------------------------------------------------------
        -- COMPLETE CHECK
        -------------------------------------------------------
        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.ShippingInstruction_Detail a
            WHERE
                a.SI_No = @InstructionNo
                AND NOT EXISTS
                (
                    SELECT 1
                    FROM dbo.PickingScan_Detail b
                    WHERE
                        b.SI_No = a.SI_No
                        AND b.Item_Code = a.Item_Code
                        AND b.Serial_No = a.Serial_No
                )
        )
        BEGIN
            UPDATE dbo.PickingScan_Header
            SET
                Picking_Status = 'COMPLETE',
                Finish_Date = @DateNow,
                Update_Date = @DateNow,
                Update_By = @UserID
            WHERE Picking_No = @PickingNo
        END

        COMMIT TRAN

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRAN

        DECLARE @Msg VARCHAR(MAX)

        SET @Msg = ERROR_MESSAGE()

        RAISERROR(@Msg,16,1)

        RETURN

    END CATCH

END
GO

