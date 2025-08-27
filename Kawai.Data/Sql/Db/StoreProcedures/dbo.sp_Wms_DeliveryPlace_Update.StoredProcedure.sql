SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE OR ALTER PROCEDURE [sp_Wms_DeliveryPlace_Update]
	@Trade_Code varchar(25),
	@Location_Code varchar(25),
	@Location_Name varchar(200),
	@UpdateBy varchar(25)
as
begin
	if not exists (select 1 from Delivery_Place where Location_Code = @Location_Code  and  Trade_Code = @Trade_Code)
	begin
		raiserror('Location Code didn''t Exists',16,1)
		return;
	end

	 

	update Delivery_Place 
	set 
		Location_Name = @Location_Name, 
		 
		Last_user = @UpdateBy, 
		Last_Update = getdate() 
	where Location_Code = @Location_Code  and  Trade_Code = @Trade_Code
end
GO
