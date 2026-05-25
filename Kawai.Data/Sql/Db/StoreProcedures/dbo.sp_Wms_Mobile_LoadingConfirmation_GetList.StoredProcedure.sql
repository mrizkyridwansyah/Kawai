CREATE PROCEDURE [dbo].[sp_Wms_Mobile_LoadingConfirmation_GetList] (
    @InstructionNo  VARCHAR(50),
    @Keyword        NVARCHAR(50) = NULL
)
AS
BEGIN

    SET NOCOUNT ON;

    -------------------------------------------------------
    -- VARIABLE
    -------------------------------------------------------
    DECLARE
        @LoadingNo             VARCHAR(50),
        @LoadingStatus         VARCHAR(30),

        @BeforeStatus          BIT = 0,
        @AfterStatus           BIT = 0,

        @TotalPlan             INT = 0,
        @TotalScanned          INT = 0,

        @IsAllScanned          BIT = 0,

        @AllowEvidenceBefore   BIT = 0,
        @AllowEvidenceAfter    BIT = 0,
        @AllowScanBarcode      BIT = 0

    -------------------------------------------------------
    -- GET HEADER
    -------------------------------------------------------
    SELECT TOP 1
        @LoadingNo     = lh.Loading_No,
        @LoadingStatus = lh.Loading_Status,
        @BeforeStatus  = lh.Before_Status,
        @AfterStatus   = lh.After_Status
    FROM dbo.LoadingConfirmationScan_Header lh
    WHERE lh.SI_No = @InstructionNo
    ORDER BY lh.Register_Date DESC

    -------------------------------------------------------
    -- TOTAL PLAN
    -------------------------------------------------------
    SELECT @TotalPlan = COUNT(*)
    FROM dbo.ShippingInstruction_Detail
    WHERE SI_No = @InstructionNo

    -------------------------------------------------------
    -- TOTAL SCANNED
    -------------------------------------------------------
    SELECT @TotalScanned = COUNT(*)
    FROM dbo.LoadingConfirmationScan_Detail
    WHERE SI_No = @InstructionNo

    -------------------------------------------------------
    -- FLAG ALL SCANNED
    -------------------------------------------------------
    --IF @TotalPlan > 0 AND @TotalPlan = @TotalScanned
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
        SET @IsAllScanned = 1
    END

    -------------------------------------------------------
    -- ALLOW EVIDENCE BEFORE
    -------------------------------------------------------
    IF ISNULL(@LoadingStatus,'OPEN') NOT IN ('CLOSED')
    BEGIN
        SET @AllowEvidenceBefore = 1
    END

    -------------------------------------------------------
    -- ALLOW EVIDENCE AFTER
    -------------------------------------------------------
    IF @BeforeStatus = 1 AND @IsAllScanned = 1 AND ISNULL(@LoadingStatus,'OPEN') <> 'CLOSED'
    BEGIN
        SET @AllowEvidenceAfter = 1
    END

    -------------------------------------------------------
    -- ALLOW SCAN BARCODE
    -------------------------------------------------------
    IF @BeforeStatus = 1 AND ISNULL(@LoadingStatus,'OPEN') NOT IN ('CLOSED')
    BEGIN
        SET @AllowScanBarcode = 1
    END

    -------------------------------------------------------
    -- RESULT
    -------------------------------------------------------
    SELECT
        PartNo = LTRIM(RTRIM(sd.Item_Code)),
        PartName = LTRIM(RTRIM(im.Item_Name)),
        SerialNo = LTRIM(RTRIM(sd.Serial_No)),
        BarcodeNo = lsd.Barcode_No,
        LoadingDate =
            CASE
                WHEN lsd.Scan_Date IS NOT NULL
                THEN CAST(lsd.Scan_Date AS DATE)
            END,
        LoadingTime =
            CASE
                WHEN lsd.Scan_Date IS NOT NULL
                THEN CAST(lsd.Scan_Date AS TIME)
            END,
        IsScanned =
            CAST(
                CASE
                    WHEN lsd.Barcode_No IS NULL
                        THEN 0
                    ELSE 1
                END
            AS BIT),
        StatusLoading = ISNULL(@LoadingStatus, 'OPEN'),

        ---------------------------------------------------
        -- SUMMARY FLAG
        ---------------------------------------------------
        TotalPlan = @TotalPlan,
        TotalScanned = @TotalScanned,
        IsAllScanned = @IsAllScanned,

        ---------------------------------------------------
        -- EVIDENCE STATUS
        ---------------------------------------------------
        BeforeStatus = @BeforeStatus,
        AfterStatus = @AfterStatus,

        ---------------------------------------------------
        -- UI CONTROL FLAG
        ---------------------------------------------------
        AllowEvidenceBefore = @AllowEvidenceBefore,
        AllowEvidenceAfter = @AllowEvidenceAfter,
        AllowScanBarcode = @AllowScanBarcode
    FROM dbo.ShippingInstruction_Detail sd
    INNER JOIN dbo.Item_Master im ON im.Item_Code = sd.Item_Code
    LEFT JOIN dbo.LoadingConfirmationScan_Detail lsd ON lsd.SI_No = sd.SI_No AND lsd.Item_Code = sd.Item_Code AND lsd.Serial_No = sd.Serial_No
    WHERE (
            (@InstructionNo <> 'ALL' AND sd.SI_No = @InstructionNo)
            OR
            (@InstructionNo = 'ALL')
        )
        AND (
            ISNULL(@Keyword, '') = ''
            OR sd.Item_Code LIKE '%' + @Keyword + '%'
            OR im.Item_Name LIKE '%' + @Keyword + '%'
            OR sd.Serial_No LIKE '%' + @Keyword + '%'
            OR lsd.Barcode_No LIKE '%' + @Keyword + '%'
        )
    ORDER BY
        CASE
            WHEN lsd.Barcode_No IS NULL THEN 0
            ELSE 1
        END,
        lsd.SeqNo,
        sd.Serial_No;
END
GO


