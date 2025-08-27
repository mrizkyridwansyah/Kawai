SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_ManufactureLine_LineDDL]
	@Keyword varchar(max),
	@ManufactureCode varchar(10)
as
select Line_Code LineCode, Line_Name LineName 
From Manufacture_Line
where 1=1
and (@ManufactureCode = 'ALL' or Manufacture_Code = @ManufactureCode)
and Line_Name like '%' + @Keyword + '%'
GO
