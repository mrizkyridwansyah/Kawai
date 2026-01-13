SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [sp_Wms_TradeDelivery_Upd]
	
	@Trade_Code 	 varchar(25) = '000006' ,
	@Location_Code Varchar(25),
	@Location_Name Varchar(100),
	@UserID varchar(25) 
as 
 
		 
		 insert into Delivery_Place(
			Trade_Code,
			Location_Code,
			Location_Name, 
			Last_Update,
			Last_User,
			Register_Date) values 
			(@Trade_Code , @Location_Code , @Location_Name,    Getdate(),@UserID ,Getdate())
	 
	 
 
 


GO
