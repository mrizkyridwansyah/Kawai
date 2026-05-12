
create   procedure [dbo].[sp_Wms_Receipt_GenerateCode]
	@FactoryCode varchar(5) = null,
	@ReceiptDate date
as
begin
	declare @prefixFactory varchar(5) = (select fak.PrefixGlobalBarcode From Company_Profile fak where fak.Company_Code = @FactoryCode)

	if @ReceiptDate < cast(getdate() as date)
	begin
		raiserror('Receipt Date tidak boleh back date!', 16, 1)
		return
	end

	DECLARE	@ReceiptNo varchar(50), @prefixReceiptNo varchar(10) = 'R.' + FORMAT(@ReceiptDate, 'yyyyMMdd') + '.'
	EXEC dbo.GenerateNumerator @Prefix = @prefixReceiptNo, @LengthSequence = 4, @Result = @ReceiptNo OUTPUT;			

	SELECT @ReceiptNo
end
