USE [EZRunnerV3_KawaiLive]
GO

/****** Object:  StoredProcedure [dbo].[sp_Wms_Mobile_PickingByScan_GetList]    Script Date: 5/19/2026 9:04:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- EXEC [dbo].[sp_Wms_Mobile_PickingByScan_GetList] 'SI-004/SCRAP/IX/20'
CREATE PROCEDURE [dbo].[sp_Wms_Mobile_PickingByScan_GetList]
(
    @InstructionNo  VARCHAR(50),
    @Keyword        NVARCHAR(50) = NULL
)
AS
BEGIN

    SET NOCOUNT ON;

    SELECT
        PartNo = LTRIM(RTRIM(sd.Item_Code)),
        PartName = LTRIM(RTRIM(im.Item_Name)),
        SerialNo = LTRIM(RTRIM(sd.Serial_No)),
        BarcodeNo = psd.Barcode_No,
        PickingDate =
            CASE
                WHEN psd.Scan_Date IS NOT NULL
                THEN CAST(psd.Scan_Date AS DATE)
            END,
        PickingTime =
            CASE
                WHEN psd.Scan_Date IS NOT NULL
                THEN CAST(psd.Scan_Date AS TIME)
            END,
        IsScanned =
            CAST(
                CASE
                    WHEN psd.Barcode_No IS NULL
                        THEN 0
                    ELSE 1
                END
            AS BIT),
        StatusPicking = ISNULL(psh.Picking_Status, 'OPEN')
    FROM dbo.ShippingInstruction_Detail sd
    INNER JOIN dbo.Item_Master im ON im.Item_Code = sd.Item_Code
    LEFT JOIN dbo.PickingScan_Detail psd ON psd.SI_No = sd.SI_No AND psd.Item_Code = sd.Item_Code AND psd.Serial_No = sd.Serial_No
    LEFT JOIN dbo.PickingScan_Header psh ON psh.Picking_No = psd.Picking_No AND psh.SI_No = psd.SI_No
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
            OR psd.Barcode_No LIKE '%' + @Keyword + '%'
        )
    ORDER BY
        CASE
            WHEN psd.Barcode_No IS NULL THEN 0
            ELSE 1
        END,
        psd.SeqNo,
        sd.Serial_No;

END
GO

