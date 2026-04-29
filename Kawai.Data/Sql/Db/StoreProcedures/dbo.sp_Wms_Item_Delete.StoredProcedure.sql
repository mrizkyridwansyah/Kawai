CREATE   PROCEDURE [dbo].[sp_Wms_Item_Delete]
	@ItemCode varchar(25)
as
begin
	if not exists (select 1 from Item_Master where Item_Code = @ItemCode)
	begin
		raiserror('Data Item didn''t Exists',16,1)
		return;
	end

	IF (select [dbo].fn_CheckItemUsage(@ItemCode)) = 0
	BEGIN
		raiserror('Data Item already used as reference!',16,1)
		return;
	END

	delete from Item_Master where Item_Code = @ItemCode
end
