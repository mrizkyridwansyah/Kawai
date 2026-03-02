SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE   procedure [sp_Wms_StopPoint_DDL]
	@Keyword varchar(max) = ''
as
begin
	select 
		RTRIM(StopPointCode) StopPointCode, [Description], RTRIM(StopPointCode) + ' | ' + [Description] as [DDLDescription]
	From MS_StopPoint
	where 1=1
	and (StopPointCode like '%'+ @Keyword +'%' or Description like '%'+ @Keyword +'%')
end

GO
