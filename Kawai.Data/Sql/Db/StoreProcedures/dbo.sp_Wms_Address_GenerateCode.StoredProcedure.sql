SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create   procedure [sp_Wms_Address_GenerateCode]
	@WarehouseCode varchar(25),
	@AreaCode varchar(25)
as
	declare @lastSN int = isnull((select max(cast(right(AddressCode, 3) as int)) from MS_Address where warehousecode = @WarehouseCode and AreaCode = @AreaCode), 0) + 1
	select rtrim(@WarehouseCode) + '/' + rtrim(@AreaCode) + '/' + right(('000' + cast(@lastSN as varchar)), 3)
GO
