SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER PROCEDURE [sp_Wms_Trade_DDL]
	@Keyword varchar(max) = '',
    @Trade_Cls varchar(max) = '2'
as
begin
	select 
		RTRIM(Trade_Code) Trade_Code, Trade_Name
	From Trade_Master
	where 1=1 and Trade_Cls = @Trade_Cls
	and Trade_Name like '%'+ @Keyword +'%'
end

GO
