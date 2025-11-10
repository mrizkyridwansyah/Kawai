SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_Privileges_CaptureAreaPrivileges]
	@UserID varchar(25)
as 

select UserID, AreaCode, AllowAccess From SS_UserAreaPrivilege where UserID = @UserID
GO
