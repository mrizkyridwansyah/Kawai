SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_Cls_DDL]
	@TypeData varchar(50),
	@Keyword varchar(max)
as
select ClsCode, [Description] From vw_Cls
where 1=1
and TypeData = @TypeData
and [Description] like '%'+ @Keyword +'%'
GO
