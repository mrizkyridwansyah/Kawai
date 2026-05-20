
-- EXEC [dbo].[sp_Wms_Mobile_PickingByScan_GetDataBarcode] 'SI-004/SCRAP/IX/20', 'K3RM202604280018'
CREATE PROCEDURE [dbo].[sp_Wms_Mobile_PickingByScan_GetDataBarcode]
(
    @InstructionNo  VARCHAR(50),
    @BarcodeNo      VARCHAR(100)
)
AS
BEGIN

    SET NOCOUNT ON;

    -------------------------------------------------------
    -- VALIDASI BARCODE NO
    -------------------------------------------------------
    IF NOT EXISTS (
        SELECT 1
        FROM dbo.ShippingInstruction_Detail sid
        JOIN dbo.StockDetail sd ON sd.ItemCode = sid.Item_Code
        WHERE sid.SI_No = @InstructionNo AND sd.BarcodeNo = @BarcodeNo
    )
    BEGIN
        RAISERROR('Nomor barcode tidak valid!',16,1)
        RETURN
    END

    DECLARE
        @WarehouseCode    VARCHAR(25),
        @AreaCode         VARCHAR(25),
        @AddressCode      VARCHAR(25),
        @ItemCode         VARCHAR(50),
        @SerialNo         VARCHAR(100),
        @PickingNo        VARCHAR(50),
        @StatusReceipt    VARCHAR(20),
        @QtyStock         NUMERIC(18,9)

    -------------------------------------------------------
    -- GET STOCK DATA
    -------------------------------------------------------
    SELECT
        @WarehouseCode = a.WarehouseCode,
        @AreaCode      = a.AreaCode,
        @AddressCode   = a.AddressCode,
        @ItemCode      = a.ItemCode,
        @SerialNo      = b.Serial_No,
        @PickingNo     = a.Picking_No,
        @StatusReceipt = a.StatusReceipt,
        @QtyStock      = ISNULL(a.Qty,0)
    FROM dbo.StockDetail a
    OUTER APPLY (
        SELECT TOP 1 x.Serial_No FROM dbo.ShippingInstruction_Detail x
        WHERE x.SI_No = @InstructionNo AND x.Item_Code = a.ItemCode
    ) b
    WHERE a.BarcodeNo = @BarcodeNo AND ISNULL(a.Qty,0) > 0

    -------------------------------------------------------
    -- VALIDASI STOCK ADA
    -------------------------------------------------------
    IF ISNULL(@QtyStock,0) <= 0
    BEGIN
        RAISERROR('Stock barcode tidak ditemukan!',16,1)
        RETURN
    END

    -------------------------------------------------------
    -- VALIDASI STATUS RECEIPT
    -------------------------------------------------------
    IF ISNULL(@StatusReceipt,'') <> 'OK'
    BEGIN
        RAISERROR('Status barcode belum OK!',16,1)
        RETURN
    END

    -------------------------------------------------------
    -- VALIDASI BARCODE SUDAH DIPICKING LAIN
    -------------------------------------------------------
    IF ISNULL(@PickingNo,'') <> ''
    BEGIN
        RAISERROR('Barcode sudah digunakan proses picking lain!',16,1)
        RETURN
    END

    -------------------------------------------------------
    -- VALIDASI ADA DI SHIPPING INSTRUCTION
    -------------------------------------------------------
    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.ShippingInstruction_Detail
        WHERE
            SI_No      = @InstructionNo
            AND Item_Code = @ItemCode
            AND Serial_No = @SerialNo
    )
    BEGIN
        RAISERROR('Serial number tidak terdaftar pada Shipping Instruction!',16,1)
        RETURN
    END

    -------------------------------------------------------
    -- VALIDASI SUDAH PERNAH DISCAN
    -------------------------------------------------------
    IF EXISTS
    (
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

    -------------------------------------------------------
    -- RETURN PREVIEW DATA
    -------------------------------------------------------
    SELECT TOP 1
        sd.WarehouseCode,
        sd.AreaCode,
        sd.AddressCode,
        sd.BarcodeNo,
        sid.SI_No AS SINo,
        sid.PO_NO AS PONo,
        sid.PO_SeqNo AS POSeqNo,
        sd.ItemCode AS PartNo,
        im.Item_Name AS PartName,
        sd.LotNo AS SerialNo,
        sd.Qty,
        sd.StatusReceipt
    FROM dbo.StockDetail sd
    INNER JOIN dbo.ShippingInstruction_Detail sid ON sid.SI_No = @InstructionNo AND sid.Item_Code = sd.ItemCode
    LEFT JOIN dbo.Item_Master im ON im.Item_Code = sd.ItemCode
    WHERE sd.BarcodeNo = @BarcodeNo AND sd.Qty > 0

END
