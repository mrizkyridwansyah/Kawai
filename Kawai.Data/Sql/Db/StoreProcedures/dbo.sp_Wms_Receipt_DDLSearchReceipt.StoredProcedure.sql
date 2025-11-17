SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Receipt_DDLSearchReceipt]
	@Keyword varchar(max) = ''
as
begin
	select Id, ReceiptNo, DNNumber From PartReceiptHeader
	where (DNNumber like '%'+@Keyword+'%' or ReceiptNo like '%'+@Keyword+'%')
	and StatusReceipt IN ('NEW', 'PENDING')
end
GO
