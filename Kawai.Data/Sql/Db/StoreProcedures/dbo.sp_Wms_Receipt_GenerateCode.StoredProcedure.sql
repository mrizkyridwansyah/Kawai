SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Receipt_GenerateCode]
as
begin

	DECLARE	@ReceiptNo varchar(50), @prefixReceiptNo varchar(10) = 'R.' + FORMAT(GETDATE(), 'yyyyMMdd') + '.'
	EXEC dbo.GenerateNumerator @Prefix = @prefixReceiptNo, @LengthSequence = 4, @Result = @ReceiptNo OUTPUT;			

	SELECT @ReceiptNo
end
GO
