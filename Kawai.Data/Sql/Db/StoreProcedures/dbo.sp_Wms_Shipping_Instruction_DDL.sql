CREATE   PROCEDURE [dbo].[sp_Wms_Shipping_Instruction_DDL]
	@Keyword varchar(max) = '',
    @SourceMenu varchar(max) = '',
 	@SupplierCode varchar(25),
	@PeriodFrom date = null,
	@PeriodUntil date = null,
	@UserId varchar(25)
as
begin

 
	SELECT DISTINCT SI_NO as ShippingInstructionNo
	FROM ShippingInstruction_Master
	WHERE (ISNULL(@SupplierCode,'ALL') = 'ALL' OR Cust_Code = @SupplierCode)
	AND CAST(SI_Date AS DATE) BETWEEN CAST(@PeriodFrom AS DATE) AND CAST(@PeriodUntil AS DATE)
	GROUP BY SI_NO

 
end
