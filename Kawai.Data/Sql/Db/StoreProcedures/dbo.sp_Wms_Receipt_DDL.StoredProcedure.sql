SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [sp_Wms_Receipt_DDL]
	@Keyword varchar(max) = '',
	@Status varchar(max) = '',
	@SourceMenu varchar(max) = '',
	@SupplierCode varchar(25),
	@PeriodFrom date = null,
	@PeriodUntil date = null
as
begin
	if @PeriodFrom is not null and @PeriodUntil is not null
	begin
		select Id, ReceiptNo from PartReceiptHeader
		where 1=1 and ReceiptNo like '%' + @Keyword + '%' 
		and ReceiptDate between @PeriodFrom and @PeriodUntil
		and 1 = case when @SupplierCode = 'ALL' then 1 when @SupplierCode = SupplierCode then 1 else 0 end 
		and StatusReceipt = @Status
		and 1 = case when isnull(@SourceMenu, '') = '' then 1 when isnull(@SourceMenu, '') = SourceMenu then 1 else 0 end
	end
	else
	begin
		select Id, ReceiptNo from PartReceiptHeader
		where 1=1 and ReceiptNo like '%' + @Keyword + '%' 
		and 1 = case when @SupplierCode = 'ALL' then 1 when @SupplierCode = SupplierCode then 1 else 0 end 
		and StatusReceipt = @Status
		and 1 = case when isnull(@SourceMenu, '') = '' then 1 when isnull(@SourceMenu, '') = SourceMenu then 1 else 0 end
	end
end
GO
