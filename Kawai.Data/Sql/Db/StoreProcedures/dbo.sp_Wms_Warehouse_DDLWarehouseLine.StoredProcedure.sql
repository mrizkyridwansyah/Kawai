SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create or alter procedure [dbo].[sp_Wms_Warehouse_DDLWarehouseLine]
	@FactoryCode varchar(25),
	@Keyword varchar(max) = ''
as
begin
	select 
		wh.WarehouseCode, wh.WarehouseName, wh.WarehouseCode + ' | ' + wh.WarehouseName DDLDescription
	From vw_WarehouseLine wh 
	where 1=1
	and (@FactoryCode = 'ALL' or wh.FactoryCode = @FactoryCode)
	and (wh.WarehouseCode like '%'+ @Keyword +'%' or wh.WarehouseName like '%'+ @Keyword +'%')

end


GO
