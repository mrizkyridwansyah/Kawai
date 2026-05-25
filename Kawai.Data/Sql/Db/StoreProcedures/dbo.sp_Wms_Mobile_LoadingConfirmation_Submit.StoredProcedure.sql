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
        @LoadingNo         VARCHAR(50),
        @ItemCode          VARCHAR(50),
        @SerialNo          VARCHAR(100),
        @Warehouse         VARCHAR(25),
        @Area              VARCHAR(25),
        @Address           VARCHAR(25),
        @Qty               NUMERIC(18,9),
        @SeqNo             INT,
        @DateNow           DATETIME = GETDATE(),
        @PO_No             VARCHAR(35),
        @PO_SeqNo          INT,
        @BeforeStatus      BIT,
        @LoadingStatus     VARCHAR(30)

    -------------------------------------------------------
    -- GET HEADER
    -------------------------------------------------------
    SELECT TOP 1
        @LoadingNo     = Loading_No,
        @BeforeStatus  = Before_Status,
        @LoadingStatus = Loading_Status
    FROM dbo.LoadingConfirmationScan_Header
    WHERE SI_No = @InstructionNo

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

    -------------------------------------------------------
    -- GET DATA
    -------------------------------------------------------
    SELECT TOP 1
        @ItemCode  = sd.ItemCode,
        @SerialNo  = sid.Serial_No,
        @Warehouse = sd.WarehouseCode,
        @Area      = sd.AreaCode,
        @Address   = sd.AddressCode,
        @Qty       = sd.Qty,
        @PO_No     = sid.PO_NO,
        @PO_SeqNo  = sid.PO_SeqNo
    FROM dbo.StockDetail sd
    INNER JOIN dbo.ShippingInstruction_Detail sid ON sid.SI_No = @InstructionNo AND sid.Item_Code = sd.ItemCode --AND sid.Serial_No = sd.LotNo
    WHERE sd.BarcodeNo = @BarcodeNo

    -------------------------------------------------------
    -- VALIDASI BARCODE
    -------------------------------------------------------
    IF @ItemCode IS NULL
    BEGIN
        RAISERROR('Barcode tidak valid!',16,1)
        RETURN
    END

    ---------------------------------------------------------
    ---- VALIDASI SUDAH LOADING
    ---------------------------------------------------------
    --IF NOT EXISTS (
    --    SELECT 1
    --    FROM dbo.LoadingConfirmationScan_Detail
    --    WHERE SI_No = @InstructionNo AND Barcode_No = @BarcodeNo
    --)
    --BEGIN
    --    RAISERROR('Barcode belum loading!',16,1)
    --    RETURN
    --END

    -------------------------------------------------------
    -- VALIDASI DOUBLE SCAN
    -------------------------------------------------------
    IF EXISTS (
        SELECT 1
        FROM dbo.LoadingConfirmationScan_Detail
        WHERE SI_No = @InstructionNo AND Barcode_No = @BarcodeNo
    )
    BEGIN
        RAISERROR('Barcode sudah pernah di-loading!',16,1)
        RETURN
    END

    BEGIN TRY

        BEGIN TRAN

        -------------------------------------------------------
        -- GENERATE SEQNO
        -------------------------------------------------------
        SELECT
            @SeqNo = ISNULL(MAX(SeqNo),0) + 1
        FROM dbo.LoadingConfirmationScan_Detail
        WHERE Loading_No = @LoadingNo

        -------------------------------------------------------
        -- INSERT DETAIL
        -------------------------------------------------------
        INSERT INTO dbo.LoadingConfirmationScan_Detail (
            Loading_No, SeqNo, SI_No, PO_No, PO_SeqNo, Item_Code, Serial_No, Barcode_No, WarehouseCode, AreaCode, AddressCode, 
            Scan_Status, Scan_Date, Scan_By, Submit_Status, Device_ID
        )
        VALUES (
            @LoadingNo, @SeqNo, @InstructionNo, @PO_No, @PO_SeqNo, @ItemCode, @SerialNo, @BarcodeNo, @Warehouse, @Area, @Address, 
            'VALID', @DateNow, @UserID, 1, @DeviceID
        )

        ---------------------------------------------------------
        ---- UPDATE STATUS LOADING
        ---------------------------------------------------------
        --UPDATE dbo.LoadingConfirmationScan_Header
        --SET
        --    Loading_Status = 'LOADING',
        --    Update_Date = @DateNow,
        --    Update_By = @UserID
        --WHERE Loading_No = @LoadingNo

        -------------------------------------------------------
        -- COMPLETE CHECK
        -------------------------------------------------------
        IF NOT EXISTS (
            SELECT 1
            FROM dbo.ShippingInstruction_Detail a
            WHERE
                a.SI_No = @InstructionNo
                AND NOT EXISTS (
                    SELECT 1
                    FROM dbo.LoadingConfirmationScan_Detail b
                    WHERE
                        b.SI_No = a.SI_No
                        AND b.Item_Code = a.Item_Code
                        AND b.Serial_No = a.Serial_No
                )
        )
        BEGIN

            UPDATE dbo.LoadingConfirmationScan_Header
            SET
                AllScanned_Status = CAST(1 AS BIT),
                --Finish_Date = @DateNow,
                Update_Date = @DateNow,
                Update_By = @UserID
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

    END CATCH

END
GO


