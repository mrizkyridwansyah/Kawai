SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   PROCEDURE [sp_Wms_StopPoint_List]
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

    -- HituStopPoint offset berdasarkan halaman
    set @offset = (@Page - 1) * @Length

	if isnull(@Sort, '') <> ''
	begin
		set @sqlSort = 'order by ' + @Sort
	end 
	else 
	begin
		set @sqlSort = 'order by wh.StopPointCode'
	end 

	declare @TotalRows int =
	(
		select count(1)
		From MS_StopPoint wh
		left join vw_User us on wh.Lastuser = us.UserID
	 	where 1=1
		and (wh.StopPointCode like '%'+@Keyword+'%' or wh.Description like '%'+@Keyword+'%')
	)

	declare @sql varchar(max) = 
	'
		select 
			wh.StopPointCode, wh.Description,case when isnull(wh.IsActive, 0) = 1 then cast(1 as bit) else cast(0 as bit) end [IsActive] ,
			wh.PickingSeq,wh.LastUpdate LastUpdate, us.FullName Lastuser, 
			'''+cast(@TotalRows as varchar)+''' as TotalRows
		From MS_StopPoint wh
		left join vw_User us on wh.Lastuser = us.UserID
	 	where 1=1
		and (wh.StopPointCode like ''%'+@Keyword+'%'' or wh.Description like ''%'+@Keyword+'%'')
		'+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)

end
GO
