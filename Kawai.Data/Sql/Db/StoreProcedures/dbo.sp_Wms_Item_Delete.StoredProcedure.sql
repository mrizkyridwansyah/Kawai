SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_Item_Delete]
	@ItemCode varchar(25)
as
begin
	if not exists (select 1 from Item_Master where Item_Code = @ItemCode)
	begin
		raiserror('Data Item didn''t Exists',16,1)
		return;
	end

	delete from Item_Master where Item_Code = @ItemCode
end
GO
