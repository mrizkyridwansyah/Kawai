SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_Area_Create]
	@WarehouseCode varchar(25),
	@AreaCode varchar(25),
	@AreaName varchar(200),
	@ItemType varchar(25),
	@RegisterBy varchar(25)
as
begin
	if not exists (select 1 from WareHouse_Master where WH_Code = @WarehouseCode)
	begin
		raiserror('Warehouse Code didn''t Exists',16,1)
		return;
	end

	if isnull(@ItemType, '') <> ''  and not exists (select 1 from ClasificationPart_Cls where ClasificationPart_Cls = @ItemType)
	begin
		raiserror('Item Classification didn''t Exists',16,1)
		return;
	end

	insert into MS_Area (warehousecode, Areacode, Areaname, ItemType, registerby, registerdate)
	values (@WarehouseCode, @AreaCode, @AreaName, @ItemType, @RegisterBy, getdate())
end
GO
