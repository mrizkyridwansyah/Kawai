CREATE PROCEDURE sp_Wms_Reprint_List_ByBarcode
(
    @tableBarcode    tvp_barcodeno  readonly
	 
)
AS
 select * from (
SELECT    C.BarcodeNo,    D.Trade_Name AS FromCompany , cp.Company_Name AS ToCompany , C.PONumber , MONTH(POH.Delivery_Date) AS ShippingLot,
C.ItemCode , TRIM(REPLACE(it.Item_Name, CHAR(9), '')) ItemName , CAST(CAST(C.Qty AS NUMERIC(18,2)) AS VARCHAR) + ' '+ISNULL(uc.Description,'') Qty ,
FORMAT(POH.Delivery_Date,'dd MMM yyyy')DeliveryDate , A.DNNumber , 
CAST(ROW_NUMBER()OVER(Partition By C.ItemCode Order By SublotNo ASC) as VARCHAR) + '/' +
CAST((Select COUNT(1) FROM PartReceiptDetailBarcode dd    where dd.ReceiptId = A.Id and DD.ItemCode = C.ItemCode) as Varchar) ShippingLabelNo
--CAST(ISNULL(C.SublotNo,1) as varchar) +'/'+ Cast((Select ISNULL(Max(SublotNo),1) from PartReceiptDetailBarcode dd where dd.ReceiptId = A.Id) as varchar) AS ShippingLabelNo  
  FROM PartReceiptHeader A LEFT JOIN  PartReceiptDetailBarcode C ON C.ReceiptId = A.Id 
LEFT JOIN Trade_Master D ON D.Trade_Code =  A.SupplierCode
LEFT JOIN PurchaseOrder_Master AS POH ON POH.PO_No = C.PONumber
LEFT JOIN Item_Master AS it ON it.Item_Code = C.ItemCode
LEFT JOIN Unit_Cls uc on uc.Unit_Cls = it.Unit_Cls
left join Company_Profile cp on cp.Company_Code = A.CompanyCode

WHERE BarcodeNo is NOT NULL  

UNION
 SELECT BC.BarcodeNo,
 D.Trade_Name AS FromCompany , 
cp.Company_Name AS ToCompany , 
C.PONumber , MONTH(POH.Delivery_Date) AS ShippingLot,
BC.Item_Code as ItemCode , TRIM(REPLACE(it.Item_Name, CHAR(9), '')) ItemName  , 
CAST(CAST(BC.Qty AS NUMERIC(18,2)) AS VARCHAR) + ' '+ISNULL(uc.Description,'') Qty , 
FORMAT(POH.Delivery_Date,'dd MMM yyyy')DeliveryDate 
, A.DNNumber , 
CAST(ROW_NUMBER()OVER(Partition By BC.Item_Code Order By BC.SublotNo ASC) as VARCHAR) + '/' +
CAST((Select COUNT(1) FROM PartReceiptDetailBarcode dd    where dd.ReceiptId = A.Id and DD.ItemCode = BC.Item_Code) as Varchar) ShippingLabelNo
   FROM Barcode_Split BC left join 
  PartReceiptDetailBarcode C ON C.BarcodeNo = BC.BarcodeNo_Original 
  Left JOIN PartReceiptHeader A ON C.ReceiptId = A.Id 
LEFT JOIN Trade_Master D ON D.Trade_Code =  A.SupplierCode
LEFT JOIN PurchaseOrder_Master AS POH ON POH.PO_No = C.PONumber
LEFT JOIN Item_Master AS it ON it.Item_Code = BC.Item_Code
LEFT JOIN Unit_Cls uc on uc.Unit_Cls = it.Unit_Cls
left join Company_Profile cp on cp.Company_Code = A.CompanyCode
where BC.BarcodeNo is NOT NULL    
) A   WHERE BarcodeNo IN (
        select BarcodeNo   from @tableBarcode
    )
