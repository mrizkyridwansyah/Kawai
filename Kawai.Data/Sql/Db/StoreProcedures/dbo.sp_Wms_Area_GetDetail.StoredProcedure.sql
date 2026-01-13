SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




create   procedure [sp_Wms_Area_GetDetail]
	@AreaCode varchar(25)
as
begin
	select WarehouseCode, AreaCode, AreaName, ItemType From MS_Area
	where AreaCode = @AreaCode
end
GO
