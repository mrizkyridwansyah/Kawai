SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [sp_Wms_ProdMaterialRequirement_Capture]
	@ParamKey varchar(100)
AS
BEGIN
	select 
		a.Factory, a.Process, a.Line, a.Model,
		LastCalculation = format(a.RegisterDate, 'dd MMM yyyy HH:mm') + ' | ' + format(a.ScheduleDate, 'dd MMM yyyy')
	From ProdMaterialReqParams a
	where ParamKey = @ParamKey

	select	
		a.*, cast(b.ProductionId as varchar(100)) ProductionId, b.Process, b.Line, b.ParentItemCode, b.ScheduleDate, b.ReqQty, b.ScanQty, b.FinalReqQty
	From ProdMaterialReqHeader a
	inner join ProdMaterialReqDetail b on a.ParamKey = b.ParamKey and a.ChildItemCode = b.ChildItemCode
	where a.ParamKey = @ParamKey
END
GO
