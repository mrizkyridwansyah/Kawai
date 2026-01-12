USE Kawaii
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Wms_Andon_Filter_DDL]
	@Keyword varchar(max) = '',
	@WarehouseCode varchar(25) = ''
AS
BEGIN
	select 
		AreaCode, AreaName
	From MS_Area
	where 1=1
	and 1 = case when isnull(@WarehouseCode, 'ALL') = 'ALL' or WarehouseCode = @WarehouseCode then 1 else 0 end
	and AreaName like '%'+ @Keyword +'%'
END
GO
