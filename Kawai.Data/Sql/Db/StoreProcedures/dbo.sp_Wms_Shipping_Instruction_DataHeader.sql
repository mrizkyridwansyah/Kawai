CREATE procedure [dbo].[sp_Wms_Shipping_Instruction_DataHeader]
	@shippinginstructionno Varchar(100)
as
begin

   select DISTINCT
 Cust_Code as Supplier
,SI_NO  as ShippingInstructionNo
,SI_Date  as ShippingInstructionDate
,PO_NO as PONumber
,PO_DelivDate as DeliveryDate
from ShippingInstruction_Master where SI_NO = @shippinginstructionno
	 
end
