SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_ManufactureLine_ManufactureDDL]
	@Keyword varchar(max)
as
	select 
		distinct a.Manufacture_Code ManufactureCode, b.Trade_Name ManufactureName, rtrim(a.Manufacture_Code) + ' | ' + b.Trade_Name DDLDescription
	From Manufacture_Line a 
	inner join Trade_Master b on a.Manufacture_Code = b.Trade_Code
	where b.Trade_Name like '%' + @Keyword + '%'
GO
