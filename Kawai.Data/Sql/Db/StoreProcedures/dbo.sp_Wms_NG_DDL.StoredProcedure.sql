SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER PROCEDURE [sp_Wms_NG_DDL]
	@Keyword varchar(max) = ''
as
begin
	select 
		RTRIM(NGCode) NGCode, Description
	From MS_NG
	where 1=1
	and Description like '%'+ @Keyword +'%'
end

GO
