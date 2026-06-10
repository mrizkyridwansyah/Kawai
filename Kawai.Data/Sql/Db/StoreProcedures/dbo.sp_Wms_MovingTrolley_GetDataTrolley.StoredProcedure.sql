CREATE procedure [dbo].[sp_Wms_MovingTrolley_GetDataTrolley]
	@TrolleyNo varchar(50)
as
begin
	if not exists (select 1 from MS_Trolley where TrolleyCode = @TrolleyNo)
	begin
		raiserror('Data Trolley tidak ditemukan!', 16, 1)
		return
	end

	if not exists (select 1 from StockDetail where RefNo = @TrolleyNo and Qty > 0)
	begin
		raiserror('Stock Trolley kosong!', 16, 1)
		return
	end

	if exists (select 1 from StockDetail where RefNo = @TrolleyNo and isnull(Picking_No, '') = '' and Qty > 0)
	begin
		raiserror('Stock Trolley tidak milik Request No!', 16, 1)
		return
	end

	SELECT 
		sd.WarehouseCode, wh.WarehouseName, sd.AreaCode, area.AreaName, sd.AddressCode, adr.AddressName, sd.Picking_No RequestNo, 
		sd.ItemCode, mi.Item_Name ItemName,
		TotalQty = isnull(sum(Qty), 0)
	FROM StockDetail sd
	left join vw_WarehouseLine wh on sd.WarehouseCode = wh.WarehouseCode
	left join vw_Area area on sd.AreaCode = area.AreaCode
	left join vw_Address adr on sd.AddressCode = adr.AddressCode
	left join Item_Master mi on sd.ItemCode = mi.Item_Code
	WHERE RefNo = @TrolleyNo and Qty > 0
	group by sd.WarehouseCode, wh.WarehouseName, sd.AreaCode, area.AreaName, sd.AddressCode, adr.AddressName, sd.Picking_No, sd.ItemCode, mi.Item_Name
end

