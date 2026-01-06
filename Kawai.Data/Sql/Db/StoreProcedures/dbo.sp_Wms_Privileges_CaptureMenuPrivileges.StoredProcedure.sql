SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [sp_Wms_Privileges_CaptureMenuPrivileges]
	@UserID varchar(25)
as 

select * From SS_UserPrivilege where UserID = @UserID
GO
