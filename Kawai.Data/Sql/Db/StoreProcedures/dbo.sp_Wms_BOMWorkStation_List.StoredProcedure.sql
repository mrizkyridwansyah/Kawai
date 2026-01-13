SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [sp_Wms_BOMWorkStation_List]
--declare
-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
    @ModelCls Varchar(100) = '',
	@ItemCode Varchar(100) = '011'
  as

  declare @sqlSort varchar(max) = ''
	declare @offset int

    -- Hitung offset berdasarkan halaman
    set @offset = (@Page - 1) * @Length

	if isnull(@Sort, '') <> ''
	begin
		set @sqlSort = 'order by ' + @Sort
	end 
	else 
	begin
		set @sqlSort = 'order by wh.WorkStationCode'
	end 

		declare @TotalRows int =
		(
			select count(1)
			   from MS_WorkStation wh
			where 1=1
			and (wh.WorkStationCode like '%'+@Keyword+'%' or wh.WorkStationName like '%'+@Keyword+'%')
		 
		)

	declare @sql varchar(max) 
	if @ModelCls = '' OR @ItemCode = ''
	begin
	set  @sql = 
	'
		select 
			WorkStationCode
            ,WorkStationName
            ,RegisterDate
			,RegisterUser
			,LastUpdate
			,LastUser, 
			'''+cast(@TotalRows as varchar)+''' as TotalRows
		From MS_WorkStation wh 
		where 1=0'
	end
	Else
	Begin
	set  @sql = 
	'
		select 
			WorkStationCode
            ,WorkStationName
            ,RegisterDate
			,RegisterUser
			,LastUpdate
			,LastUser, 
			'''+cast(@TotalRows as varchar)+''' as TotalRows
		From MS_WorkStation wh
		left join vw_User us on wh.LastUser = us.UserID
		 	where 1=1
		and (wh.WorkStationCode like ''%'+@Keyword+'%'' or wh.WorkStationName like ''%'+@Keyword+'%'')
		
		 '+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'
	End
	print @sql

	execute (@sql)

 

GO
