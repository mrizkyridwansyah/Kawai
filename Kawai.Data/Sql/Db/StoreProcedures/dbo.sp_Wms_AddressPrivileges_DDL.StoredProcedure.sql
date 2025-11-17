SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



create   procedure [sp_Wms_AddressPrivileges_DDL]
	@UserId varchar(25) = '',
	@Keyword varchar(max) = '',
	@WarehouseCode varchar(25) = '',
	@AreaCode varchar(25) = ''
as
begin
	declare @tblAreaPrivileges table (area varchar(25))

	insert into @tblAreaPrivileges
	select AreaCode From SS_UserAreaPrivilege where UserID = @UserId and AllowAccess = 1

	select 
		ma.WarehouseCode, ma.AreaCode,
		ma.AddressCode, ma.AddressName
	From MS_Address ma
	inner join @tblAreaPrivileges xx on ma.AreaCode = xx.area
	where 1=1
	and 1 = case when isnull(@WarehouseCode, '') = 'ALL' or ma.WarehouseCode = @WarehouseCode then 1 else 0 end
	and 1 = case when isnull(@AreaCode, '') = 'ALL' or ma.AreaCode = @AreaCode then 1 else 0 end
	and ma.AddressName like '%'+ @Keyword +'%'
end

GO
