SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [sp_Wms_Stock_GenerateTroliNo]
as
begin

	DECLARE	@RefNo varchar(50), @prefixReceiptNo varchar(10) = 'TRL.' + FORMAT(GETDATE(), 'yyyyMMdd') + '.'
	EXEC dbo.GenerateNumerator @Prefix = @prefixReceiptNo, @LengthSequence = 4, @Result = @RefNo OUTPUT;			

	SELECT @RefNo
end
GO
