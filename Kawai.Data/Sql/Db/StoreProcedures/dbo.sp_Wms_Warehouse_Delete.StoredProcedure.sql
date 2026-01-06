SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create   procedure [sp_Wms_Warehouse_Delete]
	@WarehouseCode varchar(25)
as
begin
	if not exists (select 1 from WareHouse_Master where WH_Code = @WarehouseCode)
	begin
		raiserror('Warehouse Code didn''t Exists',16,1)
		return;
	end

	delete from WareHouse_Master where WH_Code = @WarehouseCode
end
GO
