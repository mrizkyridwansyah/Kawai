CREATE Proc [dbo].[SP_Scheduler_TransferDataPrinter]
 
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
	 ,'VISUAL-01' PrinterName,
	 '192.168.0.25'  IP_Printer_Address	
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
FROM 
(
	SELECT    
		C.BarcodeNo,   
		D.Trade_Name AS FromCompany , 
		cp.Company_Name AS ToCompany , 
		C.PONumber , 
		MONTH(POH.Delivery_Date) AS ShippingLot,
		C.ItemCode , 
		TRIM(REPLACE(it.Item_Name, CHAR(9), '')) ItemName , 
		CAST(CAST(C.Qty AS NUMERIC(18,2)) AS VARCHAR) + ' '+ISNULL(uc.Description,'') Qty , 
		FORMAT(POH.Delivery_Date,'dd MMM yyyy')DeliveryDate , 
		A.DNNumber , 
		CAST(ROW_NUMBER()OVER(Partition By C.ItemCode Order By SublotNo ASC) as VARCHAR) + '/' +
		CAST((Select COUNT(1) FROM PartReceiptDetailBarcode dd    where dd.ReceiptId = A.Id and DD.ItemCode = C.ItemCode) as Varchar) ShippingLabelNo,
		'RECEIPT MATERIAL'BarcodeTitle,
		'RECEIPT'SourceData
	FROM PartReceiptHeader A 
	LEFT JOIN  PartReceiptDetailBarcode C ON C.ReceiptId = A.Id 
	LEFT JOIN Trade_Master D ON D.Trade_Code =  A.SupplierCode
	LEFT JOIN PurchaseOrder_Master AS POH ON POH.PO_No = C.PONumber
	LEFT JOIN Item_Master AS it ON it.Item_Code = C.ItemCode
	LEFT JOIN Unit_Cls uc on uc.Unit_Cls = it.Unit_Cls
	left join Company_Profile cp on cp.Company_Code = A.CompanyCode
	where C.BarcodeNo is NOT NULL 
	and  ISNULL(C.PrintStatus,0) = 0 
	and  C.BarcodeNo NOT IN (Select BarcodeNo from TblBarcodePrint_Update where ISNULL(PrintStatus,0) = 0) 

	UNION ALL

	SELECT 
		BC.BarcodeNo,
		D.Trade_Name AS FromCompany , 
		cp.Company_Name AS ToCompany , 
		C.PONumber , 
		MONTH(POH.Delivery_Date) AS ShippingLot,
		BC.Item_Code as ItemCode , 
		TRIM(REPLACE(it.Item_Name, CHAR(9), '')) ItemName  , 
		CAST(CAST(BC.Qty AS NUMERIC(18,2)) AS VARCHAR) + ' '+ISNULL(uc.Description,'') Qty , 
		FORMAT(POH.Delivery_Date,'dd MMM yyyy')DeliveryDate 	, 
		A.DNNumber , 
		CAST(ROW_NUMBER()OVER(Partition By BC.Item_Code Order By BC.SublotNo ASC) as VARCHAR) + '/' +
		CAST((Select COUNT(1) FROM PartReceiptDetailBarcode dd    where dd.ReceiptId = A.Id and DD.ItemCode = BC.Item_Code) as Varchar) ShippingLabelNo,
		'RECEIPT MATERIAL' BarcodeTitle,
		'BARCODESPLIT'SourceData
	FROM Barcode_Split BC 
	left join BarcodeNGDetail  ng on bc.BarcodeNo = ng.BarcodeNew   
	inner join PartReceiptDetailBarcode C ON C.BarcodeNo = BC.BarcodeNo_Original 
	Left JOIN PartReceiptHeader A ON C.ReceiptId = A.Id 
	LEFT JOIN Trade_Master D ON D.Trade_Code =  A.SupplierCode
	LEFT JOIN PurchaseOrder_Master AS POH ON POH.PO_No = C.PONumber
	LEFT JOIN Item_Master AS it ON it.Item_Code = BC.Item_Code
	LEFT JOIN Unit_Cls uc on uc.Unit_Cls = it.Unit_Cls
	left join Company_Profile cp on cp.Company_Code = A.CompanyCode
	where ng.BarcodeNew is  NULL 
	and  ISNULL(BC.Print_Cls,0) = 0 
	and  BC.BarcodeNo NOT IN (Select BarcodeNo from TblBarcodePrint_Update where ISNULL(PrintStatus,0) = 0) 

	UNION ALL

	SELECT
		bc.BarcodeNo,
		sup.Trade_Name AS FromCompany , 
		cp.Company_Name AS ToCompany , 
		ng.PONumber , 
		ShippingLot = MONTH(po.Delivery_Date),
		ng.ItemCode, 
		ItemName = TRIM(REPLACE(mi.Item_Name, CHAR(9), '')), 
		Qty = CAST(CAST(bc.Qty AS NUMERIC(18,2)) AS VARCHAR) + ' '+ISNULL(uc.Description,''), 
		DeliveryDate = FORMAT(po.Delivery_Date,'dd MMM yyyy'), 
		ng.DNNumber, 
		ShippingLabelNo = CAST(ROW_NUMBER()OVER(Partition By bc.Item_Code Order By bc.SublotNo ASC) as V