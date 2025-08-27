SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [sp_WMS_User_Search]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',
	@IsAll bit = '0',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max)
as
begin
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
		set @sqlSort = 'order by us.UserID'
	end 

	declare @TotalRows int = 
	(
		select count(1) TotalRow
		From SS_UserSetup us
		left join Cls_Parameter cp on us.JobPosition = cp.Code
		left join SS_UserSetup us2 on us.UserGroup = us2.UserID
		where 1=1
		and (us.FullName like '%'+@Keyword+'%' or cp.Description like '%'+@Keyword+'%' or us.UserID like '%'+@Keyword+'%')
	)

	declare @sql varchar(max) = 
	'
		select 
			us.UserID, us.FullName, cp.Description [JobPosition], us2.UserID [UserGroupID], isnull(us2.FullName, '''') [UserGroupName], 
			case when isnull(us.StatusAdmin, 0) = 1 then cast(1 as bit) else cast(0 as bit) end [IsAdmin], '''+cast(@TotalRows as varchar)+''' TotalRows
		From SS_UserSetup us
		left join Cls_Parameter cp on us.JobPosition = cp.Code
		left join SS_UserSetup us2 on us.UserGroup = us2.UserID
		where 1=1
		and (us.FullName like ''%'+@Keyword+'%'' or cp.Description like ''%'+@Keyword+'%'' or us.UserID like ''%'+@Keyword+'%'')
		'+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)

end
GO
