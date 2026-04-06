SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [sp_Wms_ProdMaterialRequirement_GetLastCalculate]
	@Factory varchar(25) 
as
BEGIN
	declare @paramKey varchar(100) = (select top 1 ParamKey from ProdMaterialReqParams where Factory = @Factory order by isnull(LastUpdate, RegisterDate) desc)

	select 
		a.Factory, fak.Company_Name FactoryName,  
		a.Process, CASE WHEN a.Process = 'ALL' THEN 'ALL' ELSE p.Trade_Name end ProcessName,  
		a.Line, CASE WHEN a.Line = 'ALL' THEN 'ALL' ELSE ml.Line_Name end LineName,
		a.Model, CASE WHEN a.Model = 'ALL' THEN 'ALL' ELSE model.Description end ModelName,
		LastCalculation = format(a.RegisterDate, 'dd MMM yyyy HH:mm') + ' | ' + format(a.ScheduleDate, 'dd MMM yyyy')
	From ProdMaterialReqParams a
	left join Company_Profile fak on a.Factory = fak.Company_Code
	left join Trade_Master p on a.Process = p.Trade_Code
	left join Manufacture_Line ml on a.Line = ml.Line_Code
	left join Model_Cls model on a.Model = model.Model_Cls
	where ParamKey = @ParamKey


END
GO
