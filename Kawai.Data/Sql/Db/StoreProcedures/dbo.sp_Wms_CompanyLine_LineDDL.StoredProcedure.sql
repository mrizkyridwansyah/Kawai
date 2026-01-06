SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [sp_Wms_CompanyLine_LineDDL]
	@Keyword varchar(max),
	@CompanyCode varchar(10),
	@manufacture varchar(100)
as
select Line_Code LineCode, Line_Name LineName ,trim(Line_Code) +' | '+ trim(Line_Name) DDLDescription
From Manufacture_Line A Left JOIN Trade_Master B ON A.Manufacture_Code = B.Trade_Code 
where 1=1
and (@manufacture = 'ALL' or A.Manufacture_Code = @manufacture)
and (@CompanyCode = 'ALL' or a.Company_Code = @CompanyCode)
and (Line_Name like '%' + @Keyword + '%' or Line_Code like '%' + @Keyword + '%')


GO
