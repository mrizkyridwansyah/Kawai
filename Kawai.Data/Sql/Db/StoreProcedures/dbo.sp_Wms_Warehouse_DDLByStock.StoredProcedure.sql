SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create or alter procedure [dbo].[sp_Wms_Warehouse_DDLByStock]
	@FactoryCode varchar(25),
	@Keyword		varchar(max) = '',
	@ItemCode		varchar(25)
as
begin
	select distinct a.WarehouseCode, b.WarehouseName From StockDetail a
	inner join vw_WarehouseLine b on a.WarehouseCode = b.WarehouseCode
	where 1=1
	and b.WarehouseName like '%' + @Keyword + '%'
	and (@FactoryCode = 'ALL' or b.FactoryCode = @FactoryCode)
	and (@ItemCode = 'ALL' or a.ItemCode = @ItemCode)
	and Qty > 0
end
GO
