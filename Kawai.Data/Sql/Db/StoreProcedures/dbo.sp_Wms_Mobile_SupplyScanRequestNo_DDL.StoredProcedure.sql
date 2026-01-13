SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE   procedure [sp_Wms_Mobile_SupplyScanRequestNo_DDL]
	@Keyword varchar(max),
	@LineCode varchar(10)
as
select RequestNo RequestNo
		, [Description] = 'Production Date : ' + convert(varchar, ProductionDate, 113) + ', ' + b.Line_Name
From PartMaterialRequestHeader a left join Manufacture_Line b on a.LineCode = b.Line_Code
where 1=1
and (@LineCode = 'ALL' or LineCode = @LineCode)
and RequestNo like '%' + @Keyword + '%'
 
GO
