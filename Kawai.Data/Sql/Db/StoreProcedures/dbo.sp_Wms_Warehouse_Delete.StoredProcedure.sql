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

    IF EXISTS (SELECT 1 FROM trade_master WHERE Subcon_WH_Code = @WarehouseCode)
    BEGIN
		raiserror('Data Warehouse already used as reference data',16,1)
		return;
    END

	if exists (select 1 from MS_Area where WarehouseCode = @WarehouseCode)
	begin
		raiserror('Data Warehouse already used as reference data',16,1)
		return;
	end

	if exists (select 1 from MS_Address where WarehouseCode = @WarehouseCode)
	begin
		raiserror('Data Warehouse already used as reference data',16,1)
		return;
	end

	delete from WareHouse_Master where WH_Code = @WarehouseCode
end
GO
