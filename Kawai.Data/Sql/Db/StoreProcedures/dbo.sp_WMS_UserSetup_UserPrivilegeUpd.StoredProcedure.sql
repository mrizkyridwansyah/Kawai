SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [sp_WMS_UserSetup_UserPrivilegeUpd]
	@UserID varchar(25),
	@MenuID varchar(25),
	@AllowAccess bit,
	@AllowUpdate bit,
	@AllowPrice bit
as 

	insert into SS_UserPrivilege (UserID, MenuID, AllowAccess, AllowUpdate, AllowPrice)
	values (@UserID, @MenuID, @AllowAccess, @AllowUpdate, @AllowPrice)
--delete from SS_UserPrivilege where UserID = @UserID
--delete from SS_UserWarehousePrivilege where UserID = @UserID


GO
