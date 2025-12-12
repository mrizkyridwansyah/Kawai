SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Receipt_DDLSearchReceipt]
	@Keyword varchar(max) = '',
	@UserId varchar(25)
as
begin
	select Id, ReceiptNo, DNNumber 
	From PartReceiptHeader a inner join 
	(
		select * From SS_UserFactoryPrivilege where UserID = @UserId and AllowAccess = 1
	) b on a.CompanyCode = b.FactoryCode
	where (DNNumber like '%'+@Keyword+'%' or ReceiptNo like '%'+@Keyword+'%')
	and StatusReceipt IN ('NEW', 'PENDING')
end
GO
