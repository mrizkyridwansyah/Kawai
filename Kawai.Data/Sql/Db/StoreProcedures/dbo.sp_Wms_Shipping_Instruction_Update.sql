CREATE PROCEDURE [dbo].[sp_Wms_Shipping_Instruction_Update]
	@ShippingInstructionNo		varchar(50),
	@ShippingInstructionDate	Date,
	@PONumber		varchar(50),
	@Supplier       Varchar(50),
	@Details		tvp_ShippingInstructionDetails READONLY,
	@UpdateBy		varchar(25)
as

begin
 

	 
	 SET NOCOUNT ON;

    RETURN 0; -- 0 = Success

end

