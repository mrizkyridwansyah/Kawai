

create   procedure [dbo].[sp_Wms_Mobile_SupplySubcon_GetListStock]
	@ItemCode	  VARCHAR(25)
as
begin
	select sdt.*, mw.WarehouseName, AreaName = isnull(ml.AreaName, 'Temporary'), AddressName = isnull(mx.AddressName, 'Temporary') From 
	(
		select sdx.LotNo, sdx.WarehouseCode, sdx.AddressCode, sdx.AreaCode, SUM(sdx.Qty) CurrentQty
		from StockDetail sdx
		where ItemCode = @ItemCode and Qty > 0 and isnull(sdx.Picking_No, '') = ''
		Group by sdx.LotNo, sdx.WarehouseCode, sdx.WarehouseCode, sdx.AddressCode, sdx.AreaCode
	) sdt
	left join 
	(
		SELECT WH_Code WarehouseCode, WH_Name WarehouseName fROM WareHouse_Master
		UNION ALL
		SELECT Line_Code, Line_Name FROM Manufacture_Line
	) mw on sdt.WarehouseCode = mw.WarehouseCode
	left join 
	(
		SELECT AreaCode, AreaName fROM MS_Area
		UNION ALL
		SELECT Line_Code, Line_Name FROM Manufacture_Line
	) ml on sdt.AreaCode = ml.AreaCode
	left join 
	(
		SELECT AddressCode, AddressName fROM MS_Address
		UNION ALL
		SELECT Line_Code, Line_Name FROM Manufacture_Line
	) mx on sdt.AddressCode = mx.AddressCode

end
 

