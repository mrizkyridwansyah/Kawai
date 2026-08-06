CREATE PROCEDURE [dbo].[sp_Wms_Shipping_Instruction_Capture]
	@ShippingInstructionNo		varchar(50) 
as

begin
 

	 
	  select * from ShippingInstruction_Master 
	  select * from ShippingInstruction_Detail
				 

end

