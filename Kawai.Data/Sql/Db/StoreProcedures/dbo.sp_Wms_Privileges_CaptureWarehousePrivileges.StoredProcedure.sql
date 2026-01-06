SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [sp_Wms_Privileges_CaptureWarehousePrivileges]
	@UserID varchar(25)
as 

select UserID, WarehouseCode, AllowAccess From SS_UserWarehousePrivilege where UserID = @UserID
GO
