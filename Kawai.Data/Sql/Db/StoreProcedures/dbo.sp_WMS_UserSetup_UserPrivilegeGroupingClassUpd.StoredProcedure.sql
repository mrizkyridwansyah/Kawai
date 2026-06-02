 
CREATE PROCEDURE [dbo].[sp_WMS_UserSetup_UserPrivilegeGroupingClassUpd]
	@UserID varchar(25),
	@GroupingClassPartCode varchar(25),
	@AllowAccess bit,
	@UpdateBy varchar(25)
as 

	insert into SS_UserGroupingClassPartPrivilege (UserID, GroupingClassPartCode, AllowAccess, Last_User, Last_Update)
	values (@UserID, @GroupingClassPartCode, @AllowAccess, @UpdateBy, getdate())
