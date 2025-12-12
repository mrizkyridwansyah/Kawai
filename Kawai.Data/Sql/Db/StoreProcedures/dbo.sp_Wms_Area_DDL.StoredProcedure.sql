SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER PROCEDURE [sp_Wms_Area_DDL]
	@Keyword varchar(max) = '',
	@WarehouseCode varchar(25) = ''
as
begin
	select 
		AreaCode, AreaName, AreaCode +' | '+ AreaName DDLDescription
	From MS_Area
	where 1=1
	and 1 = case when isnull(@WarehouseCode, '') = '' or WarehouseCode = @WarehouseCode then 1 else 0 end
	and (AreaCode like '%'+ @Keyword +'%' or AreaName like '%'+ @Keyword +'%')
end

GO
