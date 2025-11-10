SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [sp_WMS_UserSetup_UserPrivilegeMobileUpd]
	@UserID varchar(25),
	@MenuID varchar(25),
	@AllowAccess bit
as 

	insert into SS_UserMobilePrivilege (UserID, MenuID, AllowAccess)
	values (@UserID, @MenuID, @AllowAccess)


GO
