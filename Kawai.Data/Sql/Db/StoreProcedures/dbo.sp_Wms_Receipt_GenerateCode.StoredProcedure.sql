SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_Receipt_GenerateCode]
as
begin
	select 
		'R.' + FORMAT(GETDATE(), 'yyyyMMdd') + '.' + 
		right('0000' + cast(isnull(max(cast(right(rtrim(ReceiptNo),4) as int)), 0) + 1 as varchar), 4)
	from PartReceiptHeader where ReceiptDate = cast(getdate() as date)
end
GO
