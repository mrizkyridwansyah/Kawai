SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_Area_Update]
	@WarehouseCode varchar(25),
	@AreaCode varchar(25),
	@AreaName varchar(200),
	@ItemType varchar(25),
	@PickingSequence int,
	@UpdateBy varchar(25)
as
begin
	if not exists (select 1 from WareHouse_Master where WH_Code = @WarehouseCode)
	begin
		raiserror('Warehouse Code didn''t Exists',16,1)
		return;
	end

	if not exists (select 1 from MS_Area where Warehousecode = @WarehouseCode and Areacode = @AreaCode)
	begin
		raiserror('Area Code didn''t Exists',16,1)
		return;
	end

	if isnull(@ItemType, '') <> ''  and not exists (select 1 from ClasificationPart_Cls where ClasificationPart_Cls = @ItemType)
	begin
		raiserror('Item Classification didn''t Exists',16,1)
		return;
	end

	declare @prevItemType varchar(25) = (SELECT ItemType fROM MS_Area WHERE Warehousecode = @WarehouseCode and AreaCode = @AreaCode)

	-- kalo prev item type ga null lalu diubah dan udah ada data material request PO maka ga bisa di ubah
	if  isnull(@prevItemType, '') <> '' 
		and isnull(@prevItemType, '') <> isnull(@ItemType, '')
		and exists (select 1 from PartMaterialRequestDetail_PO where AreaCode = @AreaCode)
	begin
		raiserror('Area Code cannot change item type because the data already use in material request by bom.',16,1)
		return;
	end

	update MS_Area set Areaname = @AreaName, ItemType = @ItemType, PickingSequence = @PickingSequence, UpdateBy = @UpdateBy, UpdateDate = getdate() 
	where warehousecode = @WarehouseCode and Areacode = @AreaCode
end
GO
