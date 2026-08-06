CREATE PROCEDURE [dbo].[sp_Wms_ShippingInstruction_Report]
 --Declare
@SINo Varchar(100)='SI-KI3-15026' 



 as
Declare @Company Varchar(Max) , @Address Varchar(Max) , @Phone Varchar(Max)

select top 1 @Company = Company_Name, @Address = RTRIM(Address1) +' ' + RTRIM(Address2) + ' ' + RTRIM(City) + ' ' + RTRIM(Province)  
 + ' ' + RTRIM(Postal_Code),
@Phone = 'Phone : ' + RTRIM(Phone1)+' Fax : ' +
   RTRIM(Fax) from Company_Profile where Company_Code = '11111'


DECLARE @TblReport Table (   CompanyName VARCHAR(100) ,CompanyAddress VARCHAR(100) ,Phone VARCHAR(100) , CustPONo VARCHAR(100) ,BCType VARCHAR(100) ,BCNumber VARCHAR(100) ,Qty NUMERIC(18,5) ,Delivery VARCHAR(100) ,Model VARCHAR(100) ,VehicleNo VARCHAR(100) ,Transport VARCHAR(100) ,SJDate Date,	TanggalSurat VARCHAR(100) ,Kendaraan VARCHAR(100) ,NoKendaraan VARCHAR(100) ,ItemCode VARCHAR(100) ,ItemName VARCHAR(100) ,QtyNG Numeric(18,5),	UnitCls VARCHAR(100) ,Remarks VARCHAR(100) ,DeliveryBy VARCHAR(100) ,ApprovedBy VARCHAR(100) ,CheckedBy VARCHAR(100) ,ReceivedBy VARCHAR(100) ,DeliveryByPosition VARCHAR(100) ,ApprovedByPosition VARCHAR(100) ,CheckedByPosition VARCHAR(100) ,ReceivedByPosition VARCHAR(100))

Insert into @TblReport
   select 

  @Company CompanyName, 
  @Address CompanyAddress,  
  @Phone Phone, 
  
  A.PO_NO CustPONo ,  
  BC_Type BCType ,   
  BC40_No BCNumber, 
  0 Qty , 
   td.Trade_Name Delivery , 
  mc.Description Model, 
  '' VehicleNo , '' Transport , 
  A.SI_Date as SJDate, 
  Format(SI_Date,'dd-MMM-yyyy') as TanggalSurat, 
  '' as Kendaraan, 
  '' as NoKendaraan,
 A.Item_Code  ItemCode , 
 A.Item_Name as ItemName , 
 A.Qty QtyNG, G.[Description] as UnitCls, '' Remarks, 
'' DeliveryBy , ''ApprovedBy , ''CheckedBy , ''ReceivedBy,
'' DeliveryByPosition , ''ApprovedByPosition , ''CheckedByPosition , ''ReceivedByPosition
from ShippingInstruction_Master A LEFT JOIN PurchaseOrder_Master B ON A.PO_NO = B.PO_No
LEFT JOIN DO_Master C ON C.List_PO = B.PO_No
LEFT JOIN Item_Master E ON A.Item_Code = E.Item_Code 
 
Left JOIN Unit_Cls G ON E.Unit_Cls = G.Unit_Cls
LEFT JOIN PurchaseOrder_Master poh on poh.PO_No = A.PO_NO
left join Trade_Master td on td.Trade_Code = A.Cust_Code
 
LEFT JOIN Model_Cls mc on mc.Model_Cls = E.Model_Cls
where A.SI_NO = @SINo
 
  
   
 Select 
 ROW_NUMBER() OVER (ORDER BY ItemCode) AS [No],
 CompanyName	
 ,CompanyAddress	
 ,Phone	
 ,CustPONo	
 ,BCType	,
 @SINo as SJNo 
 ,BCNumber	
 ,CAST((select SUM(QTYNG) from @TblReport) as float) Qty	
 ,Delivery	
 ,Model	
 ,VehicleNo	
 ,Transport	
 ,SJDate	
 ,TanggalSurat	
 ,Kendaraan	
 ,NoKendaraan	
 ,ItemCode	
 ,ItemName	
 ,CAST(SUM(QtyNG) as float) QtyNG	
 ,UnitCls	
 ,Remarks	
 ,DeliveryBy	
 ,ApprovedBy	
 ,CheckedBy	
 ,ReceivedBy	
 ,DeliveryByPosition	
 ,ApprovedByPosition	
 ,CheckedByPosition	
 ,ReceivedByPosition
 
 from @TblReport Group by CompanyName	
 ,CompanyAddress	
 ,Phone	
 ,CustPONo	
 ,BCType	
 ,BCNumber	
 ,Qty	
 ,Delivery	
 ,Model	
 ,VehicleNo	
 ,Transport	
 ,SJDate	
 ,TanggalSurat	
 ,Kendaraan	
 ,NoKendaraan	
 ,ItemCode	
 ,ItemName		
 ,UnitCls	
 ,Remarks	
 ,DeliveryBy	
 ,ApprovedBy	
 ,CheckedBy	
 ,ReceivedBy	
 ,DeliveryByPosition	
 ,ApprovedByPosition	
 ,CheckedByPosition	
 ,ReceivedByPosition
 ORDER BY ItemCode;

  
 

