SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [sp_Wms_PartMaterialRequestWomin_Capture]
	@ProductionId bigint
as
begin
	select 
		pmrh.RequestID, 
		pmrh.RequestNo, 
		pmrh.RequestDate, 
		pmrh.ProductionID, 
		pmrh.LineCode, 
		pmrh.ProductionDate,
		pmrh.ParentItem_Code,
		pmrh.RequestSetQty,
		pmrh.Status, 
		pmrh.Remarks
	From PartMaterialRequestHeader pmrh 
	where pmrh.ProductionID = @ProductionId

	select 
		pmrd.RequestDetailID, 
		pmrd.RequestDetailNo, 
		pmrd.RequestID, 
		pmrd.WorkStationCode, 
		pmrd.AreaCode,
		pmrd.SEQ,
		pmrd.RackNumber,
		pmrd.RefNumber,
		pmrd.RequestStatusID,
		pmrd.Remarks
	From 
	(
		select * from PartMaterialRequestHeader where ProductionID = @ProductionId
	) pmrh 
	inner join PartMaterialRequestDetail pmrd on pmrh.RequestID = pmrd.RequestID

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
		select * from PartMaterialRequestHeader where ProductionID = @ProductionId
	) pmrh 
	inner join PartMaterialRequestDetail pmrd on pmrh.RequestID = pmrd.RequestID
	inner join PartMaterialRequestItemDetail pmrid on pmrd.RequestDetailID = pmrid.RequestDetailID
end
GO
