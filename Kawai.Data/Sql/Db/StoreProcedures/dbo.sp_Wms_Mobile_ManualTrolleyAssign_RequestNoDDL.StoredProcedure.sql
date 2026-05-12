

create   procedure [dbo].[sp_Wms_Mobile_ManualTrolleyAssign_RequestNoDDL]
	@Keyword varchar(max)='',
	@LineCode varchar(10)  ,
	@ItemClass varchar(100)  
as
begin
	select Top 1
		dtl.RefNumber RequestNo,   [Description] = 'Production : ' + convert(varchar, ProductionDate, 113) + ', ' + rtrim(ml.Line_Name) +','+ rtrim(dtl.workstationCode)
	From PartMaterialRequestHeader hd
	INNER JOIN PartMaterialRequestDetail dtl on dtl.RequestID = hd.RequestID 
	INNER JOIN Manufacture_Line ml on hd.LineCode = ml.Line_Code
	WHERE 1=1 
	AND (dtl.Trolley_No IS NULL or dtl.Trolley_No = '')
	AND (@LineCode = 'ALL' or LineCode = @LineCode)
	AND dtl.AreaCode = @ItemClass
	AND RequestNo like '%' + @Keyword + '%'
	order by dtl.RegisterDate ASC, dtl.WorkStationCode ASC
end 
