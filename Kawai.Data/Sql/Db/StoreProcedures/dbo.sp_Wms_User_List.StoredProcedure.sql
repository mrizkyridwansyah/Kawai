SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   procedure [sp_Wms_User_List]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 3,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = ''
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

	declare @sql varchar(max) = 
	'
		select 
			us.UserID, us.FullName, us.JobPosition [JobPositionCode], cp.Description [JobPositionDesc], us2.UserID [UserGroupID], isnull(us2.FullName, '''') [UserGroupName], 
			case when isnull(us.StatusAdmin, 0) = 1 then cast(1 as bit) else cast(0 as bit) end [IsAdmin], us.RegisterDate, isnull(us3.FullName, us.RegisterUser) RegisterUser, us.UpdateDate LastUpdate, isnull(us4.FullName, us.UpdateUser) LastUser, 
			COUNT(*) OVER() AS TotalRows 
		From SS_UserSetup us
		left join Cls_Parameter cp on us.JobPosition = cp.Code
		left join SS_UserSetup us2 on us.UserGroup = us2.UserID
		left join SS_UserSetup us3 on us.RegisterUser = us3.UserID
		left join SS_UserSetup us4 on us.UpdateUser = us4.UserID
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
