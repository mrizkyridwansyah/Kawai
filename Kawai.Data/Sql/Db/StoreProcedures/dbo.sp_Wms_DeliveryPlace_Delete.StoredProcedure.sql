SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



 

CREATE OR ALTER PROCEDURE [sp_Wms_DeliveryPlace_Delete]
 
	@Trade_Code varchar(25),
	@Location_Code varchar(25)
as
begin
	if not exists (select 1 from Delivery_Place where Location_Code = @Location_Code and Trade_Code = @Trade_Code)
	begin
		raiserror('Location Code didn''t Exists',16,1)
		return;
	end

	delete from  Delivery_Place where Location_Code = @Location_Code  and Trade_Code = @Trade_Code
end
GO
