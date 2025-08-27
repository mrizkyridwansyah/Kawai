SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [sp_Wms_DeliveryPlace_GetDetail]
    @Trade_Code varchar(25),
	@Location_Code varchar(25)
as
begin
	select 
		wh.Trade_Code , wh.Location_Code, wh.Location_Name,   wh.Last_Update,  wh.Register_Date,  us.FullName Last_User 
	From Delivery_Place wh
	left join vw_User us on wh.Last_user = us.UserID
	 
	where wh.Location_Code = @Location_Code and wh.Trade_Code = @Trade_Code
end
GO
