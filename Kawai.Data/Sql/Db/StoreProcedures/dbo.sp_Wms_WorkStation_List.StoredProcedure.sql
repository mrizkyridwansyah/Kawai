SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [sp_Wms_WorkStation_List]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',
	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = ''
as
begin
	declare @sqlSort varchar(max) = ''
	declare @offset int

    -- HituWorkStation offset berdasarkan halaman
    set @offset = (@Page - 1) * @Length

	if isnull(@Sort, '') <> ''
	begin
		set @sqlSort = 'order by ' + @Sort
	end 
	else 
	begin
		set @sqlSort = 'order by wh.WorkStationCode asc'
	end 

	declare @TotalRows int =
	(
		select count(1)
		From MS_WorkStation wh
		 where 1=1
		and (wh.WorkStationCode like '%'+@Keyword+'%' or wh.WorkStationName like '%'+@Keyword+'%')
	)

	declare @sql varchar(max) = 
	'
		select 
			wh.WorkStationCode, wh.WorkStationName, 
			wh.LastUpdate LastUpdate, 
			(Select Top 1 FullName From vw_User ss where wh.LastUser = ss.UserID) LastUser, 
			wh.RegisterDate, 
			(Select Top 1 FullName From vw_User ss where wh.RegisterUser = ss.UserID) RegisterUser, 
			
			'''+cast(@TotalRows as varchar)+''' as TotalRows
		From MS_WorkStation wh
		 where 1=1
		and (wh.WorkStationCode like ''%'+@Keyword+'%'' or wh.WorkStationName like ''%'+@Keyword+'%'')
		'+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)

end
GO
