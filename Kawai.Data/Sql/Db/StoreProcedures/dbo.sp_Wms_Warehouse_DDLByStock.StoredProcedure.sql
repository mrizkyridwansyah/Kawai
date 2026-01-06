SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_Warehouse_DDLByStock]
	@FactoryCode varchar(25),
	@Keyword		varchar(max) = '',
	@ItemCode		varchar(25)
as
begin
	select 
		distinct b.WarehouseCode, b.WarehouseName, b.WarehouseCode + ' | ' + b.WarehouseName DDLDescription
	From StockDetail a
	inner join vw_WarehouseLine b on a.WarehouseCode = b.WarehouseCode
	where 1=1
	and (a.WarehouseCode like '%' + @Keyword + '%' or b.WarehouseName like '%' + @Keyword + '%')
	and (@FactoryCode = 'ALL' or b.FactoryCode = @FactoryCode)
	and (@ItemCode = 'ALL' or a.ItemCode = @ItemCode)
	and Qty > 0
end
GO
