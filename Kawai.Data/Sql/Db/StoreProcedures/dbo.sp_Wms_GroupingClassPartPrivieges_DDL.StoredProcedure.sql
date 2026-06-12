
CREATE PROCEDURE [dbo].[sp_Wms_GroupingClassPartPrivieges_DDL]
	@Keyword varchar(max),
	@UserId varchar(50)
as
select 
	Grouping_Class_Part_Code GroupingClassPartCode, [Description], rtrim(Grouping_Class_Part_Code) +' | '+ [Description] DDLDescription
From Grouping_Class_Part a
inner join 
(
	select * From SS_UserGroupingClassPartPrivilege where UserID = @UserId and AllowAccess = 1
) b on a.Grouping_Class_Part_Code = b.GroupingClassPartCode
where 1=1
and 
(
	Grouping_Class_Part_Code like '%'+ @Keyword +'%' 
	or 
	[Description] like '%'+ @Keyword +'%' 
	or 
	(rtrim(Grouping_Class_Part_Code) +' | '+ [Description] like '%'+ @Keyword +'%')
)
