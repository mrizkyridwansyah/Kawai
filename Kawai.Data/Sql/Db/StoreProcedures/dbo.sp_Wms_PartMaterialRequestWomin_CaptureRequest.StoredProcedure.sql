SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [sp_Wms_PartMaterialRequestWomin_CaptureRequest]
	@RequestId bigint
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
	where pmrh.RequestID = @RequestId

	select 
		pmrd.RequestDetailID, 
		pmrd.RequestDetailNo, 
		pmrd.RequestID, 
		pmrd.WorkStationCode, 
		pmrd.AreaCode,
		pmrd.SEQ,
		pmrd.Trolley_No,
		pmrd.RefNumber,
		pmrd.RequestStatusID,
		pmrd.Remarks
	From PartMaterialRequestDetail pmrd 
	where pmrd.RequestID = @RequestId

	select 
		pmrid.IDSeq,
		pmrd.RequestID,
		pmrid.RequestDetailID,
		pmrid.ItemCode,
		pmrid.unit_Cls,
		pmrid.ChildRequirement_Qty,
		pmrid.Remarks
	From 
	(
		select * from PartMaterialRequestDetail where RequestID = @RequestId
	) pmrd 
	inner join PartMaterialRequestItemDetail pmrid on pmrd.RequestDetailID = pmrid.RequestDetailID
end
GO
