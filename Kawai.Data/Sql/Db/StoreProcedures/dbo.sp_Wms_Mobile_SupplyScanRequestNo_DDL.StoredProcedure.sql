SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [sp_Wms_Mobile_SupplyScanRequestNo_DDL]
	@Keyword varchar(max)='',
	@LineCode varchar(10),
	@WareHouseCode varchar(100)
as
begin
	select 
		bb.RefNumber RequestNo, [Description] = 'Production : ' + convert(varchar, ProductionDate, 113) + ', ' + rtrim(b.Line_Name) +','+ rtrim(bb.workstationCode)
	From PartMaterialRequestHeader a 
	inner join PartMaterialRequestDetail bb on bb.RequestID = a.RequestID 
	left join Manufacture_Line b on a.LineCode = b.Line_Code
	where 1=1 and bb.RequestStatusID<>'5'
	and (@LineCode = 'ALL' or LineCode = @LineCode)
	and bb.AreaCode = @WareHouseCode
	and RequestNo like '%' + @Keyword + '%'
end 
GO
