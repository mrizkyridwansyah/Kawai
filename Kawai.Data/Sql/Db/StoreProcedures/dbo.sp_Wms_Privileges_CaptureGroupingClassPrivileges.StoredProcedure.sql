CREATE PROCEDURE [dbo].[sp_Wms_Privileges_CaptureGroupingClassPrivileges]
	@UserID varchar(25)
as 

select UserID, GroupingClassPartCode, AllowAccess From SS_UserGroupingClassPartPrivilege where UserID = @UserID
