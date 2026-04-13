SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE  PROCEDURE [sp_Wms_Classification_ListTab]
--DECLARE
	@Page int = 1,
	@Length int = 100,
	@Sort varchar(max) = '',
	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = ''
as
begin
	declare @sqlSort varchar(max) = ''
	declare @offset int
	set @Length = 1000
    -- Hitung offset berdasarkan halaman
    set @offset = (@Page - 1) * @Length

	if isnull(@Sort, '') <> ''
	begin
		set @sqlSort = 'order by ' + @Sort
	end 
	else 
	begin
		set @sqlSort = 'order by wh.SeqNo'
	end 

	declare @TotalRows int = 1

	declare @sql varchar(max) = 
	'
		select 
			SeqNo,
TabHeader,
TableName,
Field_1,
Field_2, 
			'''+cast(@TotalRows as varchar)+''' as TotalRows
		From WMS_Tab_Classification wh
		 where 1=1
		and (wh.TableName like ''%'+@Keyword+'%'' or wh.TableName like ''%'+@Keyword+'%'')
		'+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)

end
GO
