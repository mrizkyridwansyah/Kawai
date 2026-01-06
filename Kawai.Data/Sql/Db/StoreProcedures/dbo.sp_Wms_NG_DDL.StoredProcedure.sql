SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_NG_DDL]
	@Keyword varchar(max) = ''
as
begin
	select 
		RTRIM(NGCode) NGCode, Description NGName, RTRIM(NGCode) + ' | ' + [Description] as [DDLDescription]
	From MS_NG
	where 1=1
	and (NGCode like '%'+ @Keyword +'%' or Description like '%'+ @Keyword +'%')
end

GO
