SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



create   procedure [sp_Wms_PartMaterialRequestBom_Capture]
	@PONumber varchar(50)
as
begin
	select 
		pmrh.RequestID, 
		pmrh.RequestNo, 
		pmrh.RequestDate, 
		pmrh.PO_NO, 
		pmrh.Warehouse, 
		pmrh.ProductionDate,
		pmrh.ParentItem_Code,
		pmrh.RequestSetQty,
		pmrh.Status, 
		pmrh.Remarks
	From PartMaterialRequestHeader_PO pmrh 
	where pmrh.PO_NO = @PONumber

	select 
		pmrd.RequestDetailID, 
		pmrd.RequestDetailNo, 
		pmrd.RequestID, 
		pmrd.AreaCode,
		pmrd.SEQ,
		pmrd.RackNumber,
		pmrd.RefNumber,
		pmrd.RequestStatusID,
		pmrd.Remarks
	From 
	(
		select * from PartMaterialRequestHeader_PO where PO_NO = @PONumber
	) pmrh 
	inner join PartMaterialRequestDetail_PO pmrd on pmrh.RequestID = pmrd.RequestID

	select 
		pmrid.IDSeq,
		pmrh.RequestID,
		pmrid.RequestDetailID,
		pmrid.ItemCode,
		pmrid.unit_Cls,
		pmrid.ChildRequirement_Qty,
		pmrid.Remarks
	From 
	(
		select * from PartMaterialRequestHeader_PO where PO_NO = @PONumber
	) pmrh 
	inner join PartMaterialRequestDetail_PO pmrd on pmrh.RequestID = pmrd.RequestID
	inner join PartMaterialRequestItemDetail_PO pmrid on pmrd.RequestDetailID = pmrid.RequestDetailID
end
GO
