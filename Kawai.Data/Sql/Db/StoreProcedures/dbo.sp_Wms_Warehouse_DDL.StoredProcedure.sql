SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [sp_Wms_Warehouse_DDL]
	@Keyword varchar(max) = ''
as
begin
	select 
		RTRIM(WH_Code) WarehouseCode, WH_Name WarehouseName
	From WareHouse_Master
	where 1=1
	and WH_Name like '%'+ @Keyword +'%'
end

GO
