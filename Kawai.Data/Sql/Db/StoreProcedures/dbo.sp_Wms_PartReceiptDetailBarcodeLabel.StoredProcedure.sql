CREATE   procedure [dbo].[sp_Wms_PartReceiptDetailBarcodeLabel]
--DECLARE
@ReceiptNo VARCHAR(100)='10019'
AS
SELECT     C.BarcodeNo, A.ReceiptNo , D.Trade_Name AS FromCompany , cp.Company_Name AS ToCompany , '' PONumber , MONTH(A.ReceiptDate) AS ShippingLot,
C.ItemCode , TRIM(REPLACE(it.Item_Name, CHAR(9), '')) ItemName , CAST(CAST(C.Qty AS NUMERIC(18,2)) AS VARCHAR) + ' '+ISNULL(uc.Description,'') Qty ,
FORMAT(A.ReceiptDate,'dd MMM yyyy')DeliveryDate , A.DNNumber , 
CAST(ROW_NUMBER()OVER(Partition By C.ItemCode Order By SublotNo ASC) as VARCHAR) + '/' +
CAST((Select COUNT(1) FROM PartReceiptDetailBarcode dd    where dd.ReceiptId = A.Id and DD.ItemCode = C.ItemCode) as Varchar) ShippingLabelNo
--CAST(ISNULL(C.SublotNo,1) as varchar) +'/'+ Cast((Select ISNULL(Max(SublotNo),1) from PartReceiptDetailBarcode dd where dd.ReceiptId = A.Id) as varchar) AS ShippingLabelNo  
  FROM PartReceiptHeader A LEFT JOIN  PartReceiptDetailBarcode C ON C.ReceiptId = A.Id 
LEFT JOIN Trade_Master D ON D.Trade_Code =  A.SupplierCode
LEFT JOIN PurchaseOrder_Master AS POH ON POH.PO_No = C.PONumber
LEFT JOIN Item_Master AS it ON it.Item_Code = C.ItemCode
LEFT JOIN Unit_Cls uc on uc.Unit_Cls = it.Unit_Cls
left join Company_Profile cp on cp.Company_Code = A.CompanyCode

WHERE BarcodeNo is NOT NULL and  A.Id =@ReceiptNo