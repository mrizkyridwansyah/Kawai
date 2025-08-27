SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE OR ALTER PROCEDURE [sp_Wms_DeliveryPlace_Create]
	@Trade_Code varchar(25),
	@Location_Code varchar(25),
	@Location_Name varchar(200),
	@RegisterBy varchar(25)
as
begin
	if exists (select 1 from Delivery_Place where Location_Code = @Location_Code  and  Trade_Code = @Trade_Code)
	begin
		raiserror('Location Code Already Exists',16,1)
		return;
	end

	 

	insert into Delivery_Place(Trade_Code, Location_Code, Location_Name,   Last_user,Last_Update , Register_Date)
	values (@Trade_Code,@Location_Code, @Location_Name,   @RegisterBy, getdate(), getdate())
end
GO
