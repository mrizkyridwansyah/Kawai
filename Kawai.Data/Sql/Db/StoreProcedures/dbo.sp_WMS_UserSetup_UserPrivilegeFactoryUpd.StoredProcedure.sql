SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_WMS_UserSetup_UserPrivilegeFactoryUpd]
	@UserID varchar(25),
	@FactoryCode varchar(25),
	@AllowAccess bit,
	@UpdateBy varchar(25)
as 

	insert into SS_UserFactoryPrivilege (UserID, FactoryCode, AllowAccess, Last_User, Last_Update)
	values (@UserID, @FactoryCode, @AllowAccess, @UpdateBy, getdate())
GO
