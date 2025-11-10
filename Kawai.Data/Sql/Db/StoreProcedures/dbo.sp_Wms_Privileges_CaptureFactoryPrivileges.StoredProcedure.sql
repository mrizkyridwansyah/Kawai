SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_Privileges_CaptureFactoryPrivileges]
	@UserID varchar(25)
as 

select UserID, FactoryCode, AllowAccess From SS_UserFactoryPrivilege where UserID = @UserID
GO
