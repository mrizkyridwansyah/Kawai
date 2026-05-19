ALTER PROCEDURE [dbo].[SP_Scheduler_TransferDataPrinter]
 
as
 
 SELECT 
 LTRIM(RTRIM(A.BarcodeNo)) BarcodeNo 	
 , A.ItemCode Item_Code	
 ,A.ItemName  Item_Name	
 ,A.FromCompany SupplierName	
 ,A.ToCompany CompanyName	
 ,A.PONumber PO_No	
 ,A.DNNumber DN_No	
 ,A.ShippingLot Lot_No	
 ,A.Qty Qty	
 ,'' Unit	
 ,'' Production_Date	
 , A.DeliveryDate 	Delivery_Date
 ,''Expired	
 ,'KAWAI_EPOS_01' PrinterName,
 '172.16.10.49'  IP_Printer_Address	
 ,A.ShippingLabelNo SublotNo	
 ,''Operator	
 ,''RakNumb	
 ,''Remarks	
 ,''LineCode	
 ,''Additional_Info_01	
 ,''Additional_Info_02	
 ,''Additional_Info_03	
 ,''Additional_Info_04	
 ,''Additional_Info_05	
 ,''Additional_Info_06	
, BarcodeTitle
, SourceData
 INTO #TempStudents
 FROM (
 SELECT    C.BarcodeNo,   D.Trade_Name AS FromCompany , cp.Company_Name AS ToCompany , C.PONumber , MONTH(A.ReceiptDate) AS ShippingLot,
C.ItemCode , TRIM(REPLACE(it.Item_Name, CHAR(9), '')) ItemName , CAST(CAST(C.Qty AS NUMERIC(18,2)) AS VARCHAR) + ' '+ISNULL(uc.Description,'') Qty , 
FORMAT(A.ReceiptDate,'dd MMM yyyy')DeliveryDate , A.DNNumber , 
CAST(ROW_NUMBER()OVER(Partition By C.ItemCode Order By SublotNo ASC) as VARCHAR) + '/' +
CAST((Select COUNT(1) FROM PartReceiptDetailBarcode dd    where dd.ReceiptId = A.Id and DD.ItemCode = C.ItemCode) as Varchar) ShippingLabelNo,
 'RECEIPT MATERIAL'BarcodeTitle
,'RECEIPT'SourceData
--CAST(ISNULL(C.SublotNo,1) as varchar) +'/'+ Cast((Select ISNULL(Max(SublotNo),1) from PartReceiptDetailBarcode dd where dd.ReceiptId = A.Id) as varchar) AS ShippingLabelNo  
  FROM PartReceiptHeader A LEFT JOIN  PartReceiptDetailBarcode C ON C.ReceiptId = A.Id 
LEFT JOIN Trade_Master D ON D.Trade_Code =  A.SupplierCode
LEFT JOIN PurchaseOrder_Master AS POH ON POH.PO_No = C.PONumber
LEFT JOIN Item_Master AS it ON it.Item_Code = C.ItemCode
LEFT JOIN Unit_Cls uc on uc.Unit_Cls = it.Unit_Cls
left join Company_Profile cp on cp.Company_Code = A.CompanyCode
where C.BarcodeNo is NOT NULL and  ISNULL(C.PrintStatus,0) = 0 and  C.BarcodeNo NOT IN (Select BarcodeNo from TblBarcodePrint_Update where ISNULL(PrintStatus,0) = 0) 

 
UNION ALL

SELECT BC.BarcodeNo,
 D.Trade_Name AS FromCompany , 
cp.Company_Name AS ToCompany , 
C.PONumber , MONTH(A.ReceiptDate) AS ShippingLot,
BC.Item_Code as ItemCode , TRIM(REPLACE(it.Item_Name, CHAR(9), '')) ItemName  , 
CAST(CAST(BC.Qty AS NUMERIC(18,2)) AS VARCHAR) + ' '+ISNULL(uc.Description,'') Qty , 
FORMAT(A.ReceiptDate,'dd MMM yyyy')DeliveryDate 
, A.DNNumber , 
CAST(ROW_NUMBER()OVER(Partition By BC.Item_Code Order By BC.SublotNo ASC) as VARCHAR) + '/' +
CAST((Select COUNT(1) FROM PartReceiptDetailBarcode dd    where dd.ReceiptId = A.Id and DD.ItemCode = BC.Item_Code) as Varchar) ShippingLabelNo,
 'RECEIPT MATERIAL'BarcodeTitle
,'BARCODESPLIT'SourceData
   FROM Barcode_Split BC left join 
  PartReceiptDetailBarcode C ON C.BarcodeNo = BC.BarcodeNo_Original 
  Left JOIN PartReceiptHeader A ON C.ReceiptId = A.Id 
LEFT JOIN Trade_Master D ON D.Trade_Code =  A.SupplierCode
LEFT JOIN PurchaseOrder_Master AS POH ON POH.PO_No = C.PONumber
LEFT JOIN Item_Master AS it ON it.Item_Code = BC.Item_Code
LEFT JOIN Unit_Cls uc on uc.Unit_Cls = it.Unit_Cls
left join Company_Profile cp on cp.Company_Code = A.CompanyCode
where BC.BarcodeNo is NOT NULL and  ISNULL(BC.Print_Cls,0) = 0 and  BC.BarcodeNo NOT IN (Select BarcodeNo from TblBarcodePrint_Update where ISNULL(PrintStatus,0) = 0) 


) A

  Insert into TblBarcodePrint
  Select * from #TempStudents 
  Insert into TblBarcodePrint_Update
  Select  BarcodeNo, NULL, NULL, NULL , SourceData from #TempStudents 
 
  drop table   #TempStudents

