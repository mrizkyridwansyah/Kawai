USE [EZRunnerV3_KawaiLive]
GO

/****** Object:  StoredProcedure [dbo].[sp_Wms_Mobile_PickingByScan_GetDetail]    Script Date: 5/19/2026 9:03:56 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- EXEC [dbo].[sp_Wms_Mobile_PickingByScan_GetDetail] 'SI-004/SCRAP/IX/20', 'K3RM202604280018', '811110', 'TEST00050'
CREATE PROCEDURE [dbo].[sp_Wms_Mobile_PickingByScan_GetDetail]
    @InstructionNo varchar(50),
    @BarcodeNo varchar(50),
    @PartNo varchar(50),
    @SerialNo varchar(50)
AS

SET NOCOUNT ON;

SELECT 
    [PickingNo] = LTRIM(RTRIM(ph.Picking_No)), 
    [InstructionNo] = LTRIM(RTRIM(ph.SI_No)),
    [PONo] = LTRIM(RTRIM(pd.PO_No)),
    [WarehouseCode] = LTRIM(RTRIM(ph.WH_Code)), 
    [AreaCode] = LTRIM(RTRIM(pd.AreaCode)),
    [AddressCode] = LTRIM(RTRIM(pd.AddressCode)), 
    [PartNo] = LTRIM(RTRIM(pd.Item_Code)),
    [PartName] = LTRIM(RTRIM(im.Item_Name)),
    [SerialNo] = LTRIM(RTRIM(pd.Serial_No)),
    [BarcodeNo] = LTRIM(RTRIM(pd.Barcode_No)),
    --[PickingDate] = CASE WHEN pd.Scan_Date IS NULL THEN NULL ELSE FORMAT(pd.Scan_Date, 'yyyy-MM-dd') END,
    --[PickingTime] = CASE WHEN pd.Scan_Date IS NULL THEN NULL ELSE FORMAT(pd.Scan_Date, 'hh:mm:ss tt') END
    [PickingDate] = CASE WHEN pd.Scan_Date IS NULL THEN NULL ELSE CAST(pd.Scan_Date AS DATE) END,
    [PickingTime] = CASE WHEN pd.Scan_Date IS NULL THEN NULL ELSE CAST(pd.Scan_Date AS TIME) END
FROM dbo.PickingScan_Header ph
JOIN dbo.PickingScan_Detail pd ON pd.Picking_No = ph.Picking_No AND pd.SI_No = ph.SI_No
LEFT JOIN dbo.Item_Master im ON im.Item_Code = pd.Item_Code
WHERE 
        ph.SI_NO = @InstructionNo
    AND pd.Barcode_No = @BarcodeNo 
    AND pd.Item_Code = @PartNo 
    AND pd.Serial_No = @SerialNo
GO

