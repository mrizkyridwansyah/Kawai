SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [sp_Wms_Item_GetDetail]
	@ItemCode varchar(25)
as
begin
	select
		ItemCode				= mi.Item_Code,
		ItemName				= mi.Item_Name,
		FinishGoodPartCls		= mi.FinishGoodPart_Cls,
		FinishGoodPartClsDesc	= a.[Description],
		DrawingNumber			= mi.Drawing_Number,
		WarehouseCode			= mi.WH_Code,
		WarehouseName			= wh.WarehouseName,
		Address					= mi.Address,
		SupplierCode			= mi.Supplier_Code,
		SupplierName			= tm.Trade_Name,
		DeliveryPlaceCode		= mi.Delivery_Place,
		DeliveryPlaceName		= dp.Location_Name,
		ManufactureCode			= mi.Manufacture_Code,
		ManufactureName			= tm2.Trade_Name,
		LineCode				= mi.Line_Code,
		LineName				= ml.Line_Name,
		MakerItemCode			= mi.MakerItem_Code,
		MakerItemName			= (select top 1 xx.Item_Name from Item_Master xx where xx.Item_Code = mi.MakerItem_Code),
		PartCls					= mi.Part_Cls,
		PartClsDesc				= b.Description,
		ReserveCls				= case when isnull(mi.Reserve_Cls, '02') = '01' then cast(1 as bit) else cast(0 as bit) end,
		ReserveClsDesc			= c.Description,
		SupplyCls				= case when isnull(mi.Suply_Cls, '02') = '01' then cast(1 as bit) else cast(0 as bit) end,
		SupplyClsDesc			= d.Description,
		ProvisionCls			= case when isnull(mi.Provision_Cls, '02') = '01' then cast(1 as bit) else cast(0 as bit) end,
		ProvisionClsDesc		= e.Description,
		ProductionCls			= case when isnull(mi.Production_Cls, '02') = '01' then cast(1 as bit) else cast(0 as bit) end,
		ProductionClsDesc		= f.Description,
		MaterialCls				= mi.Material_Cls,
		MaterialClsDesc			= mc.Description,
		SheetCoilCls			= mi.SheetCoil_Cls,
		SheetCoilClsDesc		= sc.Description,
		DrawingMaterialCls		= mi.DrawingMaterial_Cls,
		DrawingMaterialClsDesc	= dmc.Description,
		SurfaceTreatmentCls		= mi.SurfaceTreatment_Cls,
		SurfaceTreatmentClsDesc	= stc.Description,
		HeatTreatmentCls		= mi.HeatTreatment_Cls,
		HeatTreatmentClsDesc	= htc.Description,
		PackingStyleCls			= mi.PackingStyle_Cls,
		PackingStyleClsDesc		= pc.Description,
		GroupCls				= mi.Group_Cls,
		GroupClsDesc			= gc.Description,
		MakeBuyCls				= mi.MakeBuy_Cls,
		MakeBuyClsDesc			= h.Description,
		ControlCls				= mi.Control_Cls,
		ControlClsDesc			= cc.Description,
		ColorCls				= mi.Color_Cls,
		ColorClsDesc			= clc.Description,
		DestinationCls			= mi.Destination_Cls,
		DestinationClsDesc		= dc.Description,
		UnitCls					= mi.Unit_Cls,
		UnitClsDesc				= un.Description,
		PackingStyleMaterialCls	= mi.PackingStyleMaterial_Cls,
		PackingStyleMaterialClsDesc	= pc.Description,
		ExplosionCls			= mi.Explosion_Cls,
		ExplosionClsDesc		= i.Description,
		PersonInChargeCls		= mi.PersonInCharge_Cls,
		PersonInChargeClsDesc	= pic.Description,
		StockControlCls			= case when isnull(mi.StockControl_Cls, '02') = '01' then cast(1 as bit) else cast(0 as bit) end,
		StockControlClsDesc		= g.Description,
		SupplyIssueCls			= mi.SupplyIssue_Cls,
		SupplyIssueClsDesc		= '',
		ModelCls				= mi.Model_Cls,	
		ModelClsDesc			= mdc.Description,	
		POTypeCls				= mi.POType_Cls,	
		POTypeClsDesc			= poc.Description,	
		ClasificationPartCls	= mi.ClasificationPart_Cls,	
		ClasificationPartClsDesc= cpc.Description,	
		HSCode					= mi.HS_Code,
		HSName					= hs.HS_Code,
		NumberProducible		= mi.Number_Producible,
		ScrapWeight				= mi.Scrap_Weight,
		SurfaceOrderPointQty	= mi.Surface_OrderPointQty,
		HeatOrderPointQty		= mi.Heat_OrderPointQty,	
		SWQty					= mi.SW_Qty,	
		EWQty					= mi.EW_Qty,	
		NumberProcess			= mi.Number_Process,	
		MaterialCoefficient		= mi.Material_Coefficient,	
		ProcessCoefficient		= mi.Process_Coefficient,	
		MinLot					= mi.Min_Lot,	
		LotQty					= mi.Lot_Qty,	
		LotCoefficience			= mi.Lot_Coefficience,	
		ProductReadTime			= mi.Product_ReadTime,	
		YieldPercentage			= mi.Yield_Percentage,	
		NumberEntering			= mi.Number_Entering,	
		StandardStock			= mi.Standard_Stock,	
		SafetyStock				= mi.Safety_Stock,	
		MaxStock				= mi.Max_Stock,	
		MinStock				= mi.Min_Stock,	
		AlowanceDay				= mi.Alowance_Day,	
		DeliveryReadTime		= mi.Delivery_ReadTime,	
		OrderPointQty			= mi.OrderPoint_Qty,	
		NumberBox				= mi.Number_Box,	
		OrderPointQty			= mi.OrderPoint_Qty,	
		AccountingCode			= mi.Accounting_Code,	
		MinOrder				= mi.MinOrder,	
		SafetyStockPercentage	= mi.Safety_Stock_Percentage,	
		SAPItemCode				= mi.SAP_Item_Code,	
		TypeAccs				= mi.TypeAccs,	
		SAPItemCode				= mi.SAP_Item_Code,	
		LastUpdate				= mi.Last_Update,	
		LastUser				= mi.Last_User,	
		RegisterDate			= mi.Register_Date,	
		UseEndDay				= dbo.ConvertToDateTimeFromFuckingString(mi.Use_EndDay),
		mi.Thickness, mi.Width, mi.Length, mi.Weight, mi.GrossWeight, mi.Pitch, mi.Sample
	From Item_Master mi 
	left join 
	(
		SELECT WH_Code WarehouseCode, WH_Name WarehouseName from warehouse_master
		UNION ALL
		SELECT DISTINCT(ml.Manufacture_Code) WarehouseCode,tm.Trade_Name WarehouseName
		FROM Manufacture_Line ml INNER JOIN Trade_Master tm on ml.Manufacture_Code = tm.Trade_Code
	) wh on mi.WH_Code = wh.WarehouseCode
	left join Trade_Master tm on mi.Supplier_Code = tm.Trade_Code
	left join Delivery_Place dp on mi.Delivery_Place = dp.Location_Code
	left join Trade_Master tm2 on mi.Manufacture_Code = tm2.Trade_Code
	left join Manufacture_Line ml on mi.Line_Code = ml.Line_Code
	left join Material_Cls mc on mi.material_Cls = mc.Material_Cls
	left join SheetCoil_Cls sc on mi.SheetCoil_Cls = sc.SheetCoil_Cls
	left join DrawingMaterial_Cls dmc on mi.DrawingMaterial_Cls = dmc.DrawingMaterial_Cls
	left join SurfaceTreatment_Cls stc on mi.SurfaceTreatment_Cls = stc.SurfaceTreatment_Cls
	left join HeatTreatment_Cls htc on mi.HeatTreatment_Cls = htc.HeatTreatment_Cls
	left join PackingStyle_Cls pc on mi.PackingStyle_Cls = pc.PackingStyle_Cls
	left join Group_Cls gc on mi.Group_Cls = gc.Group_Cls
	left join Control_Cls cc on mi.Control_Cls = cc.Control_Cls
	left join Color_Cls clc on mi.Color_Cls = clc.Color_Cls
	left join Destination_Cls dc on mi.Destination_Cls = dc.Destination_Cls
	left join Unit_Cls un on mi.Unit_Cls = un.Unit_Cls
	left join PackingStyle_Cls psc on mi.PackingStyle_Cls = psc.PackingStyle_Cls
	left join PersonInCharge_Cls pic on mi.PersonInCharge_Cls = pic.PersonInCharge_Cls
	left join Model_Cls mdc on mi.Model_Cls = mdc.Model_Cls
	left join POType_Cls poc on mi.POType_Cls = poc.POType_Cls
	left join ClasificationPart_Cls cpc on mi.ClasificationPart_Cls = cpc.ClasificationPart_Cls
	left join HS_Master hs on mi.HS_Code = hs.HS_Code
	left join vw_FinishGoodCls a on mi.FinishGoodPart_Cls = a.Code
	left join vw_PartCls b on mi.Part_Cls = b.Code
	left join vw_ReserveCls c on mi.Reserve_Cls = c.Code
	left join vw_SupplyCls d on mi.Suply_Cls = d.Code
	left join vw_ProvisionCls e on mi.Provision_Cls = e.Code
	left join vw_ProductionCls f on mi.Production_Cls = f.Code
	left join vw_StockControlCls g on mi.StockControl_Cls = g.Code
	left join vw_MakeOrBuyCls h on mi.MakeBuy_Cls = h.Code
	left join vw_ExplosionCls i on mi.Explosion_Cls = i.Code
	where mi.Item_Code = @ItemCode
end
GO
