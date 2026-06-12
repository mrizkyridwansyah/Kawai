

CREATE   procedure [dbo].[sp_Wms_Mobile_ManualTrolleyAssign_LineDDL] 
	@Keyword varchar(max) ='',
	@ItemClass Varchar(max)  	 
as
	SELECT DISTINCT 
		c.Line_Code  as LineCode, Line_Name as LineName ,rtrim(Line_Code) + ' | ' + Line_Name DDLDescription 
	FROM PartMaterialRequestDetail a 
	INNER JOIN PartMaterialRequestHeader B on a.RequestID = B.RequestID 
	INNER JOIN Manufacture_Line C on c.Line_Code = B.LineCode 
	WHERE 1=1
	AND a.RequestStatusID = 5
	--AND (a.Trolley_No IS NULL or a.Trolley_No = '')
	AND a.AreaCode = @ItemClass
	AND (c.Line_Code like '%' + @Keyword + '%' or c.Line_Name like '%' + @Keyword + '%')
