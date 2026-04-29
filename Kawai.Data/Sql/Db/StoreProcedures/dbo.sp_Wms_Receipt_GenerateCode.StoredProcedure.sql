
CREATE   procedure [dbo].[sp_Wms_Receipt_GenerateCode]
	@FactoryCode varchar(5) = null
as
begin
	declare @prefixFactory varchar(5) = (select fak.PrefixGlobalBarcode From Company_Profile fak where fak.Company_Code = @FactoryCode)

	DECLARE	@ReceiptNo varchar(50), @prefixReceiptNo varchar(10) = 'R.' + FORMAT(GETDATE(), 'yyyyMMdd') + '.'
	EXEC dbo.GenerateNumerator @Prefix = @prefixReceiptNo, @LengthSequence = 4, @Result = @ReceiptNo OUTPUT;			

	SELECT @ReceiptNo
end
