SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   procedure [sp_Wms_PartMaterialRequestBom_GetListDetail]
	@WarehouseCode varchar(25),
	@NewRequest tvp_PartMaterialRequestBom READONLY
as
begin
	declare @WarehouseName varchar(100) = (select WarehouseName From vw_WarehouseLine where WarehouseCode = @WarehouseCode)

	select 
		req.RequestId, req.PONumber,  req.PODate, @WarehouseCode WarehouseCode, @WarehouseName WarehouseName, 
		ma.AreaCode, ma.AreaName,
		req.ItemCode ParentItemCode, mi.Item_Name ParentItemName,
		bom.Item_Code ChildItemCode, mic.Item_Name ChildItemName,
		bom.Qty QtyBOM, isnull(pmrh.RequestSetQty, req.RequestSetQty) QtySet, bom.Qty * isnull(pmrh.RequestSetQty, req.RequestSetQty) RequirementQty, us.FullName RegisterUser, pmrh.RegisterDate RegisterDate
	From BOM_Master bom
	inner join @NewRequest req on bom.Parent_ItemCode = req.ItemCode
	inner join Item_Master mi on req.ItemCode = mi.Item_Code
	left join Item_Master mic on bom.Item_Code = mic.Item_Code
	inner join MS_Area ma on mic.ClasificationPart_Cls = ma.ItemType
	left join PartMaterialRequestHeader_PO pmrh on pmrh.RequestID = req.RequestId
	left join SS_UserSetup us on pmrh.RegisterUser = us.UserID
end
GO
