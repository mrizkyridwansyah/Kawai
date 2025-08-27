SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [sp_Wms_Warehouse_DDLByStock]
	@Keyword		varchar(max) = '',
	@ItemCode		varchar(25)
as
begin
	select distinct a.WarehouseCode, b.WH_Name WarehouseName From StockDetail a
	inner join WareHouse_Master b on a.WarehouseCode = b.WH_Code
	where 1=1
	and b.WH_Name like '%' + @Keyword + '%'
	and (@ItemCode = 'ALL' or a.ItemCode = @ItemCode)
	and Qty > 0
end
GO
