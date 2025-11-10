SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_WMS_UserSetup_UserPrivilegeAreaUpd]
	@UserID varchar(25),
	@AreaCode varchar(25),
	@AllowAccess bit,
	@UpdateBy varchar(25)
as 

	insert into SS_UserAreaPrivilege (UserID, AreaCode, AllowAccess, Last_User, Last_Update)
	values (@UserID, @AreaCode, @AllowAccess, @UpdateBy, getdate())
GO
