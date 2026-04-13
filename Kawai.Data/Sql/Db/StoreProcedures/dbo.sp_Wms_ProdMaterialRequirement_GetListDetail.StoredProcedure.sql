SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [sp_Wms_ProdMaterialRequirement_GetListDetail]
	@Factory varchar(25)
AS
BEGIN
	declare @paramKey varchar(100) = (select top 1 ParamKey from ProdMaterialReqParams where Factory = @Factory order by RegisterDate desc)

	select 
		a.*, mi.Item_Name ChildItemName, uc.Description UnitClsName, 
		b.Line, ml.Line_Name LineName, b.ParentItemCode, pmi.Item_Name ParentItemName, b.ScheduleDate, b.FinalReqQty
	From ProdMaterialReqHeader a
	inner join ProdMaterialReqDetail b on a.ParamKey = b.ParamKey and a.ChildItemCode = b.ChildItemCode
	inner join Item_Master mi on a.ChildItemCode = mi.Item_Code
	inner join Unit_Cls uc on a.UnitCls = uc.Unit_Cls
	inner join Manufacture_Line ml on b.Line = ml.Line_Code
	inner join Item_Master pmi on b.ParentItemCode = pmi.Item_Code
	where a.ParamKey = @ParamKey
END
GO
