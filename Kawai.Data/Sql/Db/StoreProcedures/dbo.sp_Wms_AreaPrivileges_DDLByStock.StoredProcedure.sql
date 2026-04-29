







create   procedure [dbo].[sp_Wms_AreaPrivileges_DDLByStock]
	@UserId varchar(25) = '',
	@Keyword		varchar(max) = '',
	@WarehouseCode	varchar(25),
	@ItemCode		varchar(25),
	@StatusReceipt	varchar(25),
	@StatusHoldNG	varchar(50)
as
begin
	select * From 
	(
		select 'TMP' AreaCode, 'Temporary' AreaName , 'TMP | Temporary' DDLDescription
		where isnull(@WarehouseCode, '') <> ''
		union all
		select distinct a.AreaCode, b.AreaName, a.AreaCode +' | '+ b.AreaName DDLDescription 
		From StockDetail a
		inner join MS_Area b on a.AreaCode = b.AreaCode
		inner join
		(
			select AreaCode From SS_UserAreaPrivilege where UserID = @UserId and AllowAccess = 1	
		) priv on a.AreaCode = priv.AreaCode
		where 1=1
		and (@WarehouseCode = 'ALL' or a.WarehouseCode = @WarehouseCode)
		and (@ItemCode = 'ALL' or a.ItemCode = @ItemCode)
		and Qty > 0
		and (@StatusReceipt = 'ALL' or a.StatusReceipt = @StatusReceipt)
		and (@StatusHoldNG = 'ALL' or isnull(a.StatusHoldNG, '') = @StatusHoldNG)
	) res
	where 1=1
	and (AreaCode like '%' + @Keyword + '%' or AreaName like '%' + @Keyword + '%')
end
