SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create   procedure [sp_Wms_WarehousePrivileges_DDLWarehouseLine]
	@FactoryCode varchar(25),
	@UserId varchar(25) = '',
	@Keyword varchar(max) = ''
as
begin
	select 
		wh.WarehouseCode, wh.WarehouseName, wh.WarehouseCode + ' | ' + wh.WarehouseName DDLDescription
	From 
	(
		select WarehouseCode From SS_UserWarehousePrivilege where UserID = @UserId and AllowAccess = 1	
	) priv
	inner join vw_WarehouseLine wh on priv.WarehouseCode = wh.WarehouseCode
	where 1=1
	and (@FactoryCode = 'ALL' or wh.FactoryCode = @FactoryCode)
	and (wh.WarehouseCode like '%'+ @Keyword +'%' or wh.WarehouseName like '%'+ @Keyword +'%')

end

GO
