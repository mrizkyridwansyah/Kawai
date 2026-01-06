SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_Cls_DDL]
	@TypeData varchar(50),
	@Keyword varchar(max)
as
select 
	ClsCode, [Description], ClsCode +' | '+ [Description] DDLDescription
From vw_Cls
where 1=1
and TypeData = @TypeData
and (ClsCode like '%'+ @Keyword +'%' or [Description] like '%'+ @Keyword +'%')
GO
