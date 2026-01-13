SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [sp_Wms_TradeDeliveryPlace_Detail]
--Declare
	@Trade_Code 	 varchar(25)   
   
as 

select Trade_Code,
Location_Code,
Location_Name,
 A.Last_User RegisterUser,
A.Register_Date RegisterDate
from Delivery_Place A
 where Trade_Code = @Trade_Code 


 
GO
