SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create   procedure [sp_Wms_Import_AdmGroupReference]
as
begin
	select 
		RTRIM(Trade_Code) TradeCode, Trade_Name TradeName
	From Trade_Master
	where 1=1
	and Trade_Cls IN ('1', '2', '3')
end

GO
