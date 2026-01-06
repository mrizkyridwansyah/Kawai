SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





create   procedure [sp_Wms_Area_Update]
	@WarehouseCode varchar(25),
	@AreaCode varchar(25),
	@AreaName varchar(200),
	@UpdateBy varchar(25)
as
begin
	if not exists (select 1 from WareHouse_Master where WH_Code = @WarehouseCode)
	begin
		raiserror('Warehouse Code didn''t Exists',16,1)
		return;
	end

	if not exists (select 1 from MS_Area where warehousecode = @WarehouseCode and Areacode = @AreaCode)
	begin
		raiserror('Area Code didn''t Exists',16,1)
		return;
	end

	update MS_Area set Areaname = @AreaName, UpdateBy = @UpdateBy, UpdateDate = getdate() 
	where warehousecode = @WarehouseCode and Areacode = @AreaCode
end
GO
