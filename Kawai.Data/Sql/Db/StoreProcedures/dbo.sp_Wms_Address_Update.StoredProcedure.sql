SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [sp_Wms_Address_Update]
	@WarehouseCode varchar(25),
	@AreaCode varchar(25),
	@AddressCode varchar(25),
	@AddressName varchar(200),
	@UpdateBy varchar(25)
as
begin
	if not exists (select 1 from WareHouse_Master where WH_Code = @WarehouseCode)
	begin
		raiserror('Warehouse Code didn''t Exists',16,1)
		return;
	end

	if not exists (select 1 from MS_Area where warehousecode = @WarehouseCode and AreaCode = @AreaCode)
	begin
		raiserror('Area Code didn''t Exists',16,1)
		return;
	end

	if not exists (select 1 from MS_Address where warehousecode = @WarehouseCode and AreaCode = @AreaCode and AddressCode = @AddressCode)
	begin
		raiserror('Data Address didn''t Exists',16,1)
		return;
	end

	update MS_Address set AddressName = @AddressName, UpdateBy = @UpdateBy, UpdateDate = getdate() 
	where WarehouseCode = @WarehouseCode and AreaCode = @AreaCode and AddressCode = @AddressCode
end
GO
