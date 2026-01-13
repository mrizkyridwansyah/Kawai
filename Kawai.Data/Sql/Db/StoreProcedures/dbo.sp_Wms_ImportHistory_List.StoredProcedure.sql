SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [sp_Wms_ImportHistory_List]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',
	@TemplateName varchar(max)
as
begin
	declare @sqlSort varchar(max) = ''
	declare @offset int

    -- Hitung offset berdasarkan halaman
    set @offset = (@Page - 1) * @Length

	declare @TotalRows int = (select count(1) From ImportHistories where Template = @TemplateName )

	declare @sql varchar(max) = 
	'
		select 
			Id, [Key], Template, Status, FileName, ContentType, ProcessDuration, SizeFile, RowsCount, ValidRowsCount, InvalidRowsCount, Date, User,
			'''+cast(@TotalRows as varchar)+''' as TotalRows
		From ImportHistories
		where Template = '''+@TemplateName +'''
		order by [Date] desc
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)

end
GO
