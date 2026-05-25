CREATE PROCEDURE [dbo].[sp_Wms_Mobile_LoadingConfirmation_GetDetail]
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


