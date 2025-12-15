SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE OR ALTER PROCEDURE [sp_Wms_JobPostion_DDL]
	@Keyword varchar(max) = ''
as
begin
	select 
		Code as JobPositionCode,
		[Description] as JobPositionDescs, 
		Code + ' | ' + [Description] DDLDescription 
	from Cls_Parameter 
	where ParGroup ='JobPosition'	 
	and (Code like '%'+ @Keyword +'%' or [Description] like '%'+ @Keyword +'%')
end

GO
