

create   procedure [dbo].[sp_Wms_Mobile_ManualTrolleyAssign_RequestNoDDL]
	@Keyword varchar(max)='',
	@LineCode varchar(10)  ,
	@ItemClass varchar(100)  
as
begin
	select 
		dtl.RefNumber RequestNo,   
		[Description] = 'Production : ' + convert(varchar, ProductionDate, 113) + ', ' + rtrim(ml.Line_Name) +','+ rtrim(dtl.workstationCode)
	From PartMaterialRequestHeader hd
	INNER JOIN PartMaterialRequestDetail dtl on dtl.RequestID = hd.RequestID 
	INNER JOIN Manufacture_Line ml on hd.LineCode = ml.Line_Code
	WHERE 1=1 
	AND (@LineCode = 'ALL' or LineCode = @LineCode)
	AND dtl.AreaCode = @ItemClass
	AND dtl.RequestStatusID = 5
	AND RequestNo like '%' + @Keyword + '%'
	order by dtl.RegisterDate DESC, dtl.WorkStationCode ASC
end 
