SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [sp_Wms_Warehouse_Capture]
	@WarehouseCode varchar(25)
as
select * From WareHouse_Master where WH_Code = @WarehouseCode
GO
