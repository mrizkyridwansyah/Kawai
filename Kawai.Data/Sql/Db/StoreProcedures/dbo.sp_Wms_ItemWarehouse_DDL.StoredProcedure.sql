SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_ItemWarehouse_DDL]
	@Keyword varchar(max)
as
begin
	SELECT *, rtrim(WarehouseCode) + ' | ' + WarehouseName DDLDescription FROM
	(
		SELECT WH_Code WarehouseCode, WH_Name WarehouseName from warehouse_master
		UNION ALL
		SELECT DISTINCT(ml.Manufacture_Code) WarehouseCode,tm.Trade_Name WarehouseName
		FROM Manufacture_Line ml INNER JOIN Trade_Master tm on ml.Manufacture_Code = tm.Trade_Code
	) res
	WHERE (WarehouseCode LIKE '%' + @Keyword + '%' or WarehouseName LIKE '%' + @Keyword + '%')
end
GO
