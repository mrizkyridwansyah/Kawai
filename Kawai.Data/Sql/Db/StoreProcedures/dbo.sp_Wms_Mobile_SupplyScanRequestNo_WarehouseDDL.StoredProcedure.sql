CREATE  procedure [dbo].[sp_Wms_Mobile_SupplyScanRequestNo_WarehouseDDL]
	@Keyword varchar(max) 
	 
as
	select 
		Grouping_Class_Part_Code WarehouseCode , [Description] WarehouseName, rtrim(Grouping_Class_Part_Code) + ' | ' + [Description] DDLDescription
	From Grouping_Class_Part
	where 1=1
 	and ((rtrim(Grouping_Class_Part_Code) + ' | ' + [Description]) like '%' + @Keyword + '%')
