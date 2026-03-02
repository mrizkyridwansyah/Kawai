SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



create   procedure [sp_Wms_Trolley_DDL]
	@Keyword varchar(max) = ''
as
begin
	select 
		RTRIM(TrolleyCode) TrolleyCode, [Description], RTRIM(TrolleyCode) + ' | ' + [Description] as [DDLDescription]
	From MS_Trolley
	where 1=1
	and (TrolleyCode like '%'+ @Keyword +'%' or Description like '%'+ @Keyword +'%')
end

GO
