SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_PartMaterialRequestWomin_GetListDetail]
	@LineCode varchar(25),
	@NewRequest tvp_PartMaterialRequestWomin READONLY
as
begin
	declare @LineName varchar(100) = (select Line_Name From Manufacture_Line where Line_Code = @LineCode)

	select 
		req.RequestId, req.ProductionId,  req.ScheduleDate, @LineCode LineCode, @LineName LineName, 
		bomws.WorkStationCode, ws.WorkStationName, 
		req.ItemCode ParentItemCode, mi.Item_Name ParentItemName,
		bomws.ChildItem_Code ChildItemCode, mic.Item_Name ChildItemName,
		bomws.Qty QtyBOM, isnull(pmrh.RequestSetQty, req.RequestSetQty) QtySet, bomws.Qty * isnull(pmrh.RequestSetQty, req.RequestSetQty) RequirementQty, us.FullName RegisterUser, pmrh.RegisterDate RegisterDate
	From MS_BOMPerworkstation bomws
	inner join @NewRequest req on bomws.ParentItem_Code = req.ItemCode
	inner join MS_WorkStation ws on bomws.WorkStationCode = ws.WorkStationCode
	left join Item_Master mi on req.ItemCode = mi.Item_Code
	left join Item_Master mic on bomws.ChildItem_Code = mic.Item_Code
	left join PartMaterialRequestHeader pmrh on pmrh.RequestID = req.RequestId
	left join SS_UserSetup us on pmrh.RegisterUser = us.UserID
end
GO
