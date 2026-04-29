

CREATE   procedure [dbo].[sp_Wms_Area_Create]
	@WarehouseCode varchar(25),
	@AreaCode varchar(25),--input freetext bukan lagi autogenerate
	@AreaName varchar(200),
	@ItemType varchar(25),
	@PickingSequence int,
	@IPAddress varchar(200),
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

	if exists(select top 1 1 from MS_Area where AreaCode=@AreaCode)
	begin
		raiserror('Area Code already Exists',16,1)
		return;
	end

	insert into MS_Area (warehousecode, Areacode, Areaname, ItemType, PickingSequence, registerby, registerdate, IPAddress)
	values (@WarehouseCode, @AreaCode, @AreaName, @ItemType, @PickingSequence, @RegisterBy, getdate() , @IPAddress)
end
