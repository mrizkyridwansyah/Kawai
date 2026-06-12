CREATE PROCEDURE [dbo].[sp_WMS_UserSetup_GroupingClassPrivilege]
	@UserID varchar(25)
as 

select 
	a.Grouping_Class_Part_Code GroupingClassPartCode, a.[Description] GroupingClassPartDescs, b.AllowAccess
From Grouping_Class_Part a
left join 
(
	select * From SS_UserGroupingClassPartPrivilege where UserID = @UserID
) b on a.Grouping_Class_Part_Code = b.GroupingClassPartCode
