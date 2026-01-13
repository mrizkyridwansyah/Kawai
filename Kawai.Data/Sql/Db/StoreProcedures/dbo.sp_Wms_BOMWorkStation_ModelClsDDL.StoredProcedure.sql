SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE PROCEDURE [sp_Wms_BOMWorkStation_ModelClsDDL]
	@Keyword varchar(max) = ''
as
begin
	select 
		RTRIM(Model_Cls) ModelCls, [Description]
	From Model_Cls
	where 1=1
	and [Description] like '%'+ @Keyword +'%'
end

GO
