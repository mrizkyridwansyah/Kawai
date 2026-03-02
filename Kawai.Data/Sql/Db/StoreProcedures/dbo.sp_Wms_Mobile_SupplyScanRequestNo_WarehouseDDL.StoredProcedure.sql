SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--select * from ClasificationPart_Cls
CREATE procedure [sp_Wms_Mobile_SupplyScanRequestNo_WarehouseDDL]
	@Keyword varchar(max) 
	 
as
	select 
		ClasificationPart_Cls WarehouseCode , [Description] WarehouseName, rtrim(ClasificationPart_Cls) + ' | ' + [Description] DDLDescription
	From ClasificationPart_Cls
	where 1=1
 	and (ClasificationPart_Cls like '%' + @Keyword + '%' or [Description] like '%' + @Keyword + '%')
GO
