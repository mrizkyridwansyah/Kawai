USE [EZRunnerV3_KawaiLive]
GO

/****** Object:  StoredProcedure [dbo].[sp_Wms_Mobile_PickingByScan_Capture]    Script Date: 5/19/2026 9:02:58 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_Wms_Mobile_PickingByScan_Capture]
(
    @InstructionNo VARCHAR(50),
    @BarcodeNo     VARCHAR(100)
)
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE
        @ItemCode        VARCHAR(50),
        @PlanQty         INT = 0,
        @ScanQty         INT = 0,
        @PalletNo        VARCHAR(50)

    --------------------------------------------------------
    -- GET ITEM DARI BARCODE
    --------------------------------------------------------
    SELECT TOP 1
        @ItemCode = ItemCode
    FROM dbo.StockDetail
    WHERE BarcodeNo = @BarcodeNo AND ISNULL(Qty,0) > 0

    --------------------------------------------------------
    -- TOTAL TARGET
    --------------------------------------------------------
    SELECT
        @PlanQty = COUNT(*)
    FROM dbo.ShippingInstruction_Detail
    WHERE
        SI_No = @InstructionNo
        AND Item_Code = @ItemCode

    --------------------------------------------------------
    -- TOTAL SCAN
    --------------------------------------------------------
    SELECT
        @ScanQty = COUNT(*)
    FROM dbo.PickingScan_Detail
    WHERE
        SI_No = @InstructionNo
        AND Item_Code = @ItemCode

    --------------------------------------------------------
    -- RESULT
    --------------------------------------------------------
    SELECT
        sd.WarehouseCode,
        sd.AreaCode,
        sd.AddressCode,

        sid.SI_No,

        (
            SELECT COUNT(DISTINCT RefNo)
            FROM ShippingInstructionPallet
            WHERE ShippingNo = @InstructionNo
        ) AS TotalPallet,

        sd.BarcodeNo,

        sd.ItemCode,

        im.Item_Name AS ItemName,

        sd.LotNo AS Serial_No,

        sd.Qty,

        ISNULL(@PlanQty,0) AS PlanQty,

        ISNULL(@ScanQty,0) AS ScanQty,

        (
            ISNULL(@PlanQty,0)
            - ISNULL(@ScanQty,0)
        ) AS RemainingQty,

        CASE
            WHEN ISNULL(@ScanQty,0) = 0
                THEN 'OPEN'

            WHEN @ScanQty < @PlanQty
                THEN 'PARTIAL'

            WHEN @ScanQty >= @PlanQty
                THEN 'COMPLETE'

            ELSE 'UNKNOWN'
        END AS PickingStatus,

        (
            SELECT COUNT(*)
            FROM dbo.PickingScan_Detail
            WHERE SI_No = @InstructionNo
        ) AS TotalScanAllItem

    FROM dbo.StockDetail sd

    INNER JOIN dbo.ShippingInstruction_Detail sid
        ON sid.SI_No = @InstructionNo
        AND sid.Item_Code = sd.ItemCode
        AND sid.Serial_No = sd.LotNo

    LEFT JOIN dbo.Item_Master im
        ON im.Item_Code = sd.ItemCode

    WHERE
        sd.BarcodeNo = @BarcodeNo
        AND sd.Qty >= 0

END
GO

