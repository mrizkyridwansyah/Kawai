CREATE PROCEDURE [dbo].[sp_Wms_Mobile_LoadingConfirmation_Submit]
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
        @LoadingNo          VARCHAR(50),
        @ItemCode           VARCHAR(50),
        @SerialNo           VARCHAR(100),
        @Warehouse          VARCHAR(25),
        @Area               VARCHAR(25),
        @Address            VARCHAR(25),
        @Qty                NUMERIC(18,9),
        @SeqNo              INT,
        @DateNow            DATETIME = GETDATE(),
        @PO_No              VARCHAR(35),
        @PO_SeqNo           INT,
        @BeforeStatus       BIT,
        @LoadingStatus      VARCHAR(30),
        @RefNo              VARCHAR(50),
        @LotNo              VARCHAR(100)

    -------------------------------------------------------
    -- GET HEADER
    -------------------------------------------------------
    SELECT TOP 1
        @LoadingNo     = lh.Loading_No,
        @BeforeStatus  = lh.Before_Status,
        @LoadingStatus = lh.Loading_Status
    FROM dbo.LoadingConfirmationScan_Header lh
    WHERE lh.SI_No = @InstructionNo
    ORDER BY lh.Register_Date DESC

    -------------------------------------------------------
    -- VALIDASI HEADER
    -------------------------------------------------------
    IF @LoadingNo IS NULL
    BEGIN
        RAISERROR('Loading header tidak ditemukan!',16,1)
        RETURN
    END

    -------------------------------------------------------
    -- VALIDASI BEFORE
    -------------------------------------------------------
    IF ISNULL(@BeforeStatus,0) = 0
    BEGIN
        RAISERROR('Evidence Before belum complete!',16,1)
        RETURN
    END

    -------------------------------------------------------
    -- VALIDASI CLOSED
    -------------------------------------------------------
    IF ISNULL(@LoadingStatus,'OPEN') = 'CLOSED'
    BEGIN
        RAISERROR('Loading sudah selesai!',16,1)
        RETURN
    END

    BEGIN TRY

        BEGIN TRAN

        -------------------------------------------------------
        -- VALIDASI DOUBLE SCAN (LOCK)
        -------------------------------------------------------
        IF EXISTS (
            SELECT 1
            FROM dbo.LoadingConfirmationScan_Detail WITH (UPDLOCK, HOLDLOCK)
            WHERE SI_No = @InstructionNo
            AND Barcode_No = @BarcodeNo
        )
        BEGIN
            RAISERROR('Barcode sudah pernah di-loading!',16,1)
            ROLLBACK TRAN
            RETURN
        END

        -------------------------------------------------------
        -- GET DATA STOCK HASIL PICKING
        -------------------------------------------------------
        SELECT TOP 1
            @ItemCode  = sd.ItemCode,
            @SerialNo  = sid.Serial_No,
            @Warehouse = sd.WarehouseCode,
            @Area      = sd.AreaCode,
            @Address   = sd.AddressCode,
            @Qty       = sd.Qty,
            @LotNo     = sd.LotNo,
            @RefNo     = sd.RefNo,
            @PO_No     = sid.PO_NO,
            @PO_SeqNo  = sid.PO_SeqNo
        FROM dbo.StockDetail sd WITH (UPDLOCK, HOLDLOCK)
        INNER JOIN dbo.ShippingInstructionPallet sip ON sip.RefNo = sd.RefNo AND sip.ShippingNo = @InstructionNo
        INNER JOIN dbo.ShippingInstruction_Detail sid ON sid.SI_No = @InstructionNo AND sid.Item_Code = sd.ItemCode
            --AND sid.Serial_No = sd.Serial_No
        WHERE sd.BarcodeNo = @BarcodeNo AND sd.Qty > 0
        ORDER BY sd.LastUpdate DESC

        -------------------------------------------------------
        -- VALIDASI BARCODE
        -------------------------------------------------------
        IF @ItemCode IS NULL
        BEGIN
            RAISERROR('Barcode tidak valid atau belum dipicking!',16,1)
            ROLLBACK TRAN
            RETURN
        END

        -------------------------------------------------------
        -- VALIDASI STOCK
        -------------------------------------------------------
        IF ISNULL(@Qty,0) <= 0
        BEGIN
            RAISERROR('Stock barcode sudah habis!',16,1)
            ROLLBACK TRAN
            RETURN
        END

        -------------------------------------------------------
        -- GENERATE SEQNO
        -------------------------------------------------------
        SELECT
            @SeqNo = ISNULL(MAX(SeqNo),0) + 1
        FROM dbo.LoadingConfirmationScan_Detail WITH (UPDLOCK, HOLDLOCK)
        WHERE Loading_No = @LoadingNo

        -------------------------------------------------------
        -- PROSES STOCK ISSUE
        -------------------------------------------------------
        BEGIN

            -------------------------------------------------------
            -- HISTORY OUT
            -------------------------------------------------------
            INSERT INTO dbo.ReceiptSupplyHistory (
                [Status],
                ProcessMenu,
                RefNo,
                WarehouseCode,
                AreaCode,
                AddressCode,
                ItemCode,
                BarcodeNo,
                LotNo,
                QtyTrans,
                Remarks,
                ReferenceNo,
                LogDate,
                UserID
            )
            VALUES (
                'OUT',
                'Loading Confirmation Mobile',
                @RefNo,
                @Warehouse,
                @Area,
                @Address,
                @ItemCode,
                @BarcodeNo,
                @LotNo,
                @Qty,
                'Loading ke Container',
                @InstructionNo,
                @DateNow,
                @UserID
            )

            -------------------------------------------------------
            -- UPDATE STOCK HEADER
            -------------------------------------------------------
            EXEC dbo.sp_Wms_Stock_UpSertStockHeader
                @TransDate     = @DateNow,
                @RefNo         = @RefNo,
                @WarehouseCode = @Warehouse,
                @AreaCode      = @Area,
                @ItemCode      = @ItemCode,
                @LotNo         = @LotNo,
                @QtyTrans      = @Qty,
                @InventoryQty  = NULL,
                @Type          = 'S',
                @UserId        = @UserID

            -------------------------------------------------------
            -- UPDATE STOCK DETAIL
            -------------------------------------------------------
            EXEC dbo.sp_Wms_Stock_UpSertStockDetail
                @RefNo          = @RefNo,
                @WarehouseCode  = @Warehouse,
                @AreaCode       = @Area,
                @AddressCode    = @Address,
                @ItemCode       = @ItemCode,
                @BarcodeNo      = @BarcodeNo,
                @LotNo          = @LotNo,
                @QtyAfter       = 0,
                @InventoryQty   = NULL,
                @SublotNo       = NULL,
                @UserId         = @UserID,
                @StatusReceipt  = 'OK',
                @StatusHoldNG   = NULL

        END

        -------------------------------------------------------
        -- INSERT LOADING DETAIL
        -------------------------------------------------------
        INSERT INTO dbo.LoadingConfirmationScan_Detail (
            Loading_No, SeqNo, SI_No, PO_No, PO_SeqNo, Item_Code, Serial_No, Barcode_No, WarehouseCode, AreaCode, AddressCode, 
            Scan_Status, Scan_Date, Scan_By, Submit_Status, Device_ID
        )
        VALUES (
            @LoadingNo, @SeqNo, @InstructionNo, @PO_No, @PO_SeqNo, @ItemCode, @SerialNo, @BarcodeNo, @Warehouse, @Area, @Address, 
            'VALID', @DateNow, @UserID, 1, @DeviceID
        )

        -------------------------------------------------------
        -- COMPLETE CHECK
        -- BASED ON PICKING RESULT
        -------------------------------------------------------
        IF NOT EXISTS (
            SELECT 1
            FROM dbo.PickingScan_Detail p
            WHERE p.SI_No = @InstructionNo
            AND NOT EXISTS (
                SELECT 1
                FROM dbo.LoadingConfirmationScan_Detail l
                WHERE
                    l.SI_No = p.SI_No
                    AND l.Item_Code = p.Item_Code
                    AND l.Serial_No = p.Serial_No
            )
        )
        BEGIN
            UPDATE dbo.LoadingConfirmationScan_Header
            SET
                AllScanned_Status = CAST(1 AS BIT),
                --Finish_Date       = @DateNow,
                --Loading_Status    = 'CLOSED',
                Update_Date       = @DateNow,
                Update_By         = @UserID
            WHERE Loading_No = @LoadingNo
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


