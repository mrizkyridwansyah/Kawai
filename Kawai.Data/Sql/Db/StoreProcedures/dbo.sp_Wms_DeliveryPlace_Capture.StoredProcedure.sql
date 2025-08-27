SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER PROCEDURE [sp_Wms_DeliveryPlace_Capture]
    @Trade_Code varchar(25),
	@Location_Code varchar(25)
as
select * From Delivery_Place where Location_Code = @Location_Code  and  Trade_Code = @Trade_Code
GO
