SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_WMS_UserSetup_UserPrivilegeDelete]
	@UserID varchar(25)
as 

delete from SS_UserPrivilege where UserID = @UserID
delete from SS_UserMobilePrivilege where UserID = @UserID
delete from SS_UserFactoryPrivilege where UserID = @UserID
delete from SS_UserWarehousePrivilege where UserID = @UserID
delete from SS_UserAreaPrivilege where UserID = @UserID


GO
