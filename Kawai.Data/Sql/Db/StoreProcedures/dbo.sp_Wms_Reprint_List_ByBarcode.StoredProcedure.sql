CREATE PROCEDURE [dbo].[sp_Wms_Reprint_List_ByBarcode]
(
    @tableBarcode tvp_barcodeno READONLY
)
AS
BEGIN
    -- Mencegah pesan 'X rows affected' agar network traffic lebih ringan
    SET NOCOUNT ON;

    -- [1] QUERY UNTUK BARCODE NORMAL
    SELECT 
        C.BarcodeNo, 
        D.Trade_Name AS FromCompany, 
        cp.Company_Name AS ToCompany, 
        C.PONumber, 
        MONTH(POH.Delivery_Date) AS ShippingLot,
        C.ItemCode, 
        TRIM(REPLACE(it.Item_Name, CHAR(9), '')) AS ItemName, 
        CAST(CAST(C.Qty AS NUMERIC(18,2)) AS VARCHAR) + ' ' + ISNULL(uc.Description,'') AS Qty,
        FORMAT(POH.Delivery_Date, 'dd MMM yyyy') AS DeliveryDate, 
        A.DNNumber, 
        CAST(Seq.RowSeq AS VARCHAR) + '/' + CAST(Total.TotSeq AS VARCHAR) AS ShippingLabelNo
    FROM @tableBarcode TB
    -- Filter dari awal (Early Filtering) dengan INNER JOIN ke TVP Parameter
    INNER JOIN PartReceiptDetailBarcode C ON C.BarcodeNo = TB.BarcodeNo
    LEFT JOIN PartReceiptHeader A ON A.Id = C.ReceiptId 
    LEFT JOIN Trade_Master D ON D.Trade_Code = A.SupplierCode
    LEFT JOIN PurchaseOrder_Master POH ON POH.PO_No = C.PONumber
    LEFT JOIN Item_Master it ON it.Item_Code = C.ItemCode
    LEFT JOIN Unit_Cls uc ON uc.Unit_Cls = it.Unit_Cls
    LEFT JOIN Company_Profile cp ON cp.Company_Code = A.CompanyCode
    -- Menghitung urutan label (menggantikan ROW_NUMBER agar index-friendly)
    OUTER APPLY (
        SELECT COUNT(1) AS RowSeq 
        FROM PartReceiptDetailBarcode sq 
        WHERE sq.ItemCode = C.ItemCode AND sq.SublotNo <= C.SublotNo
    ) Seq
    -- Menghitung total label dalam 1 receipt
    OUTER APPLY (
        SELECT COUNT(1) AS TotSeq 
        FROM PartReceiptDetailBarcode dd 
        WHERE dd.ReceiptId = A.Id AND dd.ItemCode = C.ItemCode
    ) Total

    UNION ALL ---------------------------------------------------------

    -- [2] QUERY UNTUK BARCODE SPLIT
    SELECT 
        BC.BarcodeNo,
        D.Trade_Name AS FromCompany, 
        cp.Company_Name AS ToCompany, 
        C.PONumber, 
        MONTH(POH.Delivery_Date) AS ShippingLot,
        BC.Item_Code AS ItemCode, 
        TRIM(REPLACE(it.Item_Name, CHAR(9), '')) AS ItemName,  
        CAST(CAST(BC.Qty AS NUMERIC(18,2)) AS VARCHAR) + ' ' + ISNULL(uc.Description,'') AS Qty, 
        FORMAT(POH.Delivery_Date, 'dd MMM yyyy') AS DeliveryDate, 
        A.DNNumber, 
        CAST(Seq.RowSeq AS VARCHAR) + '/' + CAST(Total.TotSeq AS VARCHAR) AS ShippingLabelNo
    FROM @tableBarcode TB
    -- Filter dari awal (Early Filtering) dengan INNER JOIN ke TVP Parameter
    INNER JOIN Barcode_Split BC ON BC.BarcodeNo = TB.BarcodeNo 
    LEFT JOIN PartReceiptDetailBarcode C ON C.BarcodeNo = BC.BarcodeNo_Original 
    LEFT JOIN PartReceiptHeader A ON A.Id = C.ReceiptId 
    LEFT JOIN Trade_Master D ON D.Trade_Code = A.SupplierCode
    LEFT JOIN PurchaseOrder_Master POH ON POH.PO_No = C.PONumber
    LEFT JOIN Item_Master it ON it.Item_Code = BC.Item_Code
    LEFT JOIN Unit_Cls uc ON uc.Unit_Cls = it.Unit_Cls
    LEFT JOIN Company_Profile cp ON cp.Company_Code = A.CompanyCode
    -- Menghitung urutan label
    OUTER APPLY (
        SELECT COUNT(1) AS RowSeq 
        FROM Barcode_Split sq 
        WHERE sq.Item_Code = BC.Item_Code AND sq.SublotNo <= BC.SublotNo
    ) Seq
    -- Menghitung total label dalam 1 receipt
    OUTER APPLY (
        SELECT COUNT(1) AS TotSeq 
        FROM PartReceiptDetailBarcode dd 
        WHERE dd.ReceiptId = A.Id AND dd.ItemCode = BC.Item_Code
    ) Total;
END
