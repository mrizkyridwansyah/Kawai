




CREATE   PROCEDURE [dbo].[sp_Wms_AreaPrivileges_DDL]
	@UserId varchar(25) = '',
	@Keyword varchar(max) = '',
	@WarehouseCode varchar(25) = ''
as
begin
	select 
		a.AreaCode, a.AreaName, a.AreaCode +' | '+ a.AreaName DDLDescription
	From MS_Area a
	inner join
	(
		select AreaCode From SS_UserAreaPrivilege where UserID = @UserId and AllowAccess = 1	
	) b on a.AreaCode = b.AreaCode
	where 1=1
	and 1 = case when isnull(@WarehouseCode, '') = '' or WarehouseCode = @WarehouseCode then 1 else 0 end
	and (a.AreaCode like '%'+ @Keyword +'%' or a.AreaName like '%'+ @Keyword +'%')

end

