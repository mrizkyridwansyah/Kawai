CREATE Proc [dbo].[sp_Wms_NGClaimMaterial_Report]
@FactoryCode Varchar(100)='',
@ClaimId bigint = 1

as
Declare @Company Varchar(Max) , @Address Varchar(Max) , @Phone Varchar(Max)

select top 1 @Company = Company_Name, @Address = RTRIM(Address1) +' ' + RTRIM(Address2) + ' ' + RTRIM(City) + ' ' + RTRIM(Province)  
 + ' ' + RTRIM(Postal_Code),
@Phone = 'Phone : ' + RTRIM(Phone1)+' Fax : ' +
   RTRIM(Fax) from Company_Profile

   
Select @Company CompanyName, @Address CompanyAddress,  @Phone Phone, '' [No], A.SupplierName CustPONo ,  BCType ,   BCNumber, 0 Qty , '' Delivery , '' Model, VehicleNo , Transport , ClaimDate as SJDate, Format(ClaimDate,'dd-MMM-yyyy') as TanggalSurat, E.Description as Kendaraan, VehicleNo as NoKendaraan,
B.ItemCode , C.Item_Name as ItemName , B.QtyNG, D.[Description] as UnitCls, '' Remarks,
'' DeliveryBy , ''ApprovedBy , ''CheckedBy , ''ReceivedBy,
'' DeliveryByPosition , ''ApprovedByPosition , ''CheckedByPosition , ''ReceivedByPosition
from MaterialNGClaimHeader A 
Left JOIN MaterialNGClaimDetail B ON A.ClaimID = B.ClaimID
Left JOIN Item_Master C ON C.Item_Code = B.ItemCode
Left JOIN Unit_Cls D ON C.Unit_Cls = D.Unit_Cls
Left JOIN Transport_Cls E ON E.Transport_Cls = A.Transport
where A.ClaimID = @ClaimId
 

 