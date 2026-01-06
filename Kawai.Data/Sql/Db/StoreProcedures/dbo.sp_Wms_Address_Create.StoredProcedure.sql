SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_Address_Create]
	@WarehouseCode varchar(25),
	@AreaCode varchar(25),
	@AddressCode varchar(25),
	@AddressName varchar(200),
	@RegisterBy varchar(25)
as
begin
	if not exists (select 1 from WareHouse_Master where WH_Code = @WarehouseCode)
	begin
		raiserror('Warehouse Code didn''t Exists',16,1)
		return;
	end

	if not exists (select 1 from MS_Area where WarehouseCode = @WarehouseCode and AreaCode = @AreaCode)
	begin
		raiserror('Area Code didn''t Exists',16,1)
		return;
	end

	insert into MS_Address(WarehouseCode, AreaCode, AddressCode, AddressName, RegisterBy, RegisterDate)
	values (@WarehouseCode, @AreaCode, @AddressCode, @AddressName, @RegisterBy, getdate())
end
GO
