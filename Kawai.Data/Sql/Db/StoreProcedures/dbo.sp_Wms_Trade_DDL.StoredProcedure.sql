SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER PROCEDURE [sp_Wms_Trade_DDL]
	@Keyword varchar(max) = ''
as
begin
	select 
		RTRIM(Trade_Code) Trade_Code, Trade_Name, Trade_Cls, RTRIM(Trade_Code) + ' | ' + Trade_Name DDLDescription
	From Trade_Master
	where 1=1
	and (Trade_Code like '%'+ @Keyword +'%' or Trade_Name like '%'+ @Keyword +'%')
end

GO
