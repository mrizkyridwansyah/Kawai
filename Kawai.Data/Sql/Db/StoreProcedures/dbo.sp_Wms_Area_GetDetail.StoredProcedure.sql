



CREATE   PROCEDURE [dbo].[sp_Wms_Area_GetDetail]
	@AreaCode varchar(25)
as
begin
	select WarehouseCode, AreaCode, AreaName, ItemType, PickingSequence ,IPAddress From MS_Area
	where AreaCode = @AreaCode
end
