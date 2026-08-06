CREATE PROCEDURE [dbo].[sp_Wms_Shipping_Instruction_GenerateCode]
as
begin

	DECLARE	@SINo varchar(50), @prefixSINo varchar(12) = 'SI-KI3-' + FORMAT(GETDATE(), 'yyyyMMdd') + '.'
	EXEC dbo.GenerateNumerator @Prefix = @prefixSINo, @LengthSequence = 4, @Result = @SINo OUTPUT;			

	SELECT @SINo  
end
