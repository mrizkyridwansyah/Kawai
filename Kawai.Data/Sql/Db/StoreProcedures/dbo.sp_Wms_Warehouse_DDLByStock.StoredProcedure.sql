SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [sp_Wms_Warehouse_DDLByStock]
	@FactoryCode varchar(25),
	@UserId varchar(25) = '',
	@Keyword		varchar(max) = '',
	@ItemCode		varchar(25)
as
begin
	select distinct a.WarehouseCode, b.WarehouseName From StockDetail a
	inner join 
	(
		select wh.* From 
		(
			select WarehouseCode From SS_UserWarehousePrivilege where UserID = @UserId and AllowAccess = 1	
		) priv
		inner join vw_WarehouseLine wh on priv.WarehouseCode = wh.WarehouseCode
	) b on a.WarehouseCode = b.WarehouseCode
	where 1=1
	and b.WarehouseName like '%' + @Keyword + '%'
	and (@FactoryCode = 'ALL' or b.FactoryCode = @FactoryCode)
	and (@ItemCode = 'ALL' or a.ItemCode = @ItemCode)
	and Qty > 0
end
GO
