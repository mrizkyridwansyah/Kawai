SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   PROCEDURE [sp_Wms_ImportHistory_Detail]
	-- PARAMETER WAJIB
	@Id varchar(50)
as
begin
	select 			
		Id, [Key], Template, Status, FileName, ContentType, ProcessDuration, SizeFile, RowsCount, ValidRowsCount, InvalidRowsCount, Date, User
	from ImportHistories 
	where Id = @Id

end
GO
