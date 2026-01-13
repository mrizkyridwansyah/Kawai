SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [sp_WMS_TradeDelivery_Delete]
	@Trade_Code 	 varchar(25) = '000006' 
as 

 Delete From Delivery_Place where Trade_Code = @Trade_Code
GO
