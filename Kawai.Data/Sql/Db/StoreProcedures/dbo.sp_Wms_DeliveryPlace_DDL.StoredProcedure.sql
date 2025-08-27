SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE OR ALTER PROCEDURE [sp_Wms_DeliveryPlace_DDL]
    @Trade_Code varchar(25),
	@Keyword varchar(max) = ''
as
begin
	select 
		RTRIM(Location_Code) Location_Code, Location_Name
	From Delivery_Place
	where 1=1 and Trade_Code = @Trade_Code
	and Location_Name like '%'+ @Keyword +'%'
end

GO
