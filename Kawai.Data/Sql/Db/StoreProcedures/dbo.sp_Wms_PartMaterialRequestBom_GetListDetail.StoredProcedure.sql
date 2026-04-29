


create   procedure [dbo].[sp_Wms_PartMaterialRequestBom_GetListDetail]
	@WarehouseCode varchar(25),
	@NewRequest tvp_PartMaterialRequestBom READONLY
as
begin
	declare @tblScan table (RequestDetailId bigint, ItemCode varchar(25), TotalScan numeric(18,9))

	if exists (select * From @NewRequest where RequestId is not null)
	begin
		insert into @tblScan
		select pmrd.RequestDetailID, pmrids.ItemCode, sum(pmrids.Qty) TotalScan 
		From PartMaterialRequestItemDetailScan_PO pmrids
		inner join PartMaterialRequestItemDetail_PO pmrid on pmrids.IDSeq = pmrid.IDSeq
		inner join PartMaterialRequestDetail_PO pmrd on pmrid.RequestDetailID = pmrd.RequestDetailID
		inner join @NewRequest req on pmrd.RequestID = req.RequestId
		group by pmrd.RequestDetailID, pmrids.ItemCode
	end

	declare @WarehouseName varchar(100) = (select WarehouseName From vw_WarehouseLine where WarehouseCode = @WarehouseCode)

	select 
		req.RequestId, req.PONumber,  req.PODate, @WarehouseCode WarehouseCode, @WarehouseName WarehouseName, 
		isnull(mic.ClasificationPart_Cls, '20') ClassificationCode, cls.Description ClassificationName,
		req.ItemCode ParentItemCode, mi.Item_Name ParentItemName,
		bom.Item_Code ChildItemCode, mic.Item_Name ChildItemName,
		bom.Qty QtyBOM, 
		isnull(pmrh.RequestSetQty, req.RequestSetQty) QtySet, 
		bom.Qty * isnull(pmrh.RequestSetQty, req.RequestSetQty) RequirementQty, 
		reqCls.StatusDescription [Status], isnull(scan.TotalScan, 0) TotalScan, pmrd.RefNumber PickingNo,
		us.FullName RegisterUser, pmrh.RegisterDate RegisterDate
	From BOM_Master bom
	inner join @NewRequest req on bom.Parent_ItemCode = req.ItemCode
	inner join Item_Master mi on req.ItemCode = mi.Item_Code
	left join Item_Master mic on bom.Item_Code = mic.Item_Code
	inner join ClasificationPart_Cls cls on isnull(mic.ClasificationPart_Cls, '20') = cls.ClasificationPart_Cls
	left join PartMaterialRequestHeader_PO pmrh on pmrh.RequestID = req.RequestId
	LEFT JOIN PartMaterialRequestDetail_PO pmrd
		ON pmrd.RequestID = pmrh.RequestID and pmrd.AreaCode = isnull(mic.ClasificationPart_Cls, '20')
	left join SS_UserSetup us on pmrh.RegisterUser = us.UserID
	LEFT JOIN RequestStatusCls reqCls on pmrd.RequestStatusID = reqCls.RequestStatusID
	LEFT JOIN @tblScan scan on pmrd.RequestDetailID = scan.RequestDetailId and bom.Item_Code = scan.ItemCode
end
