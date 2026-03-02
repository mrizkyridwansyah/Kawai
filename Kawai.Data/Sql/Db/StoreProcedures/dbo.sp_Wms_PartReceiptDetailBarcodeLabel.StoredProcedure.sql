SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [sp_Wms_PartReceiptDetailBarcodeLabel]
--DECLARE
@ReceiptNo VARCHAR(100)='10019'
AS
SELECT     C.BarcodeNo, A.ReceiptNo , D.Trade_Name AS FromCompany , cp.Company_Name AS ToCompany , C.PONumber , MONTH(POH.Delivery_Date) AS ShippingLot,
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

WHERE BarcodeNo is NOT NULL and  A.Id =@ReceiptNo
--SELECT * FROM PartReceiptDetailBarcode AS prdb WHERE prdb.ReceiptId = '10009'

--SELECT * FROM PurchaseOrder_Master AS pom WHERE pom.PO_No='KI3-250121'
GO
