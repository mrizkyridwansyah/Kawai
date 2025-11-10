SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




create   procedure [sp_Wms_AreaPrivileges_DDLByStock]
	@UserId varchar(25) = '',
	@Keyword		varchar(max) = '',
	@WarehouseCode	varchar(25),
	@ItemCode		varchar(25)
as
begin
	select * From 
	(
		select 'TMP' AreaCode, 'Temporary' AreaName 
		where isnull(@WarehouseCode, '') <> ''
		union all
		select distinct a.AreaCode, b.AreaName From StockDetail a
		inner join MS_Area b on a.AreaCode = b.AreaCode
		inner join
		(
			select AreaCode From SS_UserAreaPrivilege where UserID = @UserId and AllowAccess = 1	
		) priv on a.AreaCode = priv.AreaCode
		where 1=1
		and a.WarehouseCode = @WarehouseCode
		and (@ItemCode = 'ALL' or a.ItemCode = @ItemCode)
		and Qty > 0
	) res
	where 1=1
	and AreaName like '%' + @Keyword + '%'
end
GO
