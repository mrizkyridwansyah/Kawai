SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--select * from ClasificationPart_Cls
CREATE procedure [sp_Wms_Mobile_SupplyScanRequestNo_LineDDL]
 
	@Keyword varchar(max) ='',
	@warehousecode Varchar(max)  
	 
 as
 


	select distinct c.Line_Code  as LineCode, Line_Name as LineName ,rtrim(Line_Code) + ' | ' + Line_Name DDLDescription 
	from PartMaterialRequestDetail a Left JOIN PartMaterialRequestHeader B on a.RequestID = B.RequestID Left JOIN Manufacture_Line C on c.Line_Code = B.LineCode where RequestStatusID <>'5' and AreaCode = @warehousecode
	and (Line_Code like '%' + @Keyword + '%' or Line_Name like '%' + @Keyword + '%')
GO
