SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




create   procedure [sp_Wms_AddressPrivileges_DDLByStock]
	@UserId varchar(25) = '',
	@Keyword		varchar(max) = '',
	@WarehouseCode	varchar(25),
	@AreaCode		varchar(25),
	@ItemCode		varchar(25)
as
begin
	declare @tblAreaPrivileges table (area varchar(25))

	insert into @tblAreaPrivileges
	select AreaCode From SS_UserAreaPrivilege where UserID = @UserId and AllowAccess = 1

	select 
		distinct 
		a.AddressCode, b.AddressName,
		a.AddressCode + ' | ' + b.AddressName DDLDescription
	From StockDetail a
	inner join MS_Address b on a.AddressCode = b.AddressCode
	inner join @tblAreaPrivileges xx on a.AreaCode = xx.area
	where 1=1
	and a.WarehouseCode = @WarehouseCode
	and a.AreaCode = @AreaCode
	and (@ItemCode = 'ALL' or a.ItemCode = @ItemCode)
	and Qty > 0
	and (a.AddressCode like '%'+ @Keyword +'%' or b.AddressName like '%'+ @Keyword +'%')

end
GO
