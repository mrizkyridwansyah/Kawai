SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create   procedure [sp_Wms_Area_GenerateCode]
	@WarehouseCode varchar(25)
as
	declare @lastSN int = isnull((select max(cast(right(Areacode, 3) as int)) from MS_Area where warehousecode = @WarehouseCode), 0) + 1
	select rtrim(@WarehouseCode) + '/' + right(('000' + cast(@lastSN as varchar)), 3)
GO
