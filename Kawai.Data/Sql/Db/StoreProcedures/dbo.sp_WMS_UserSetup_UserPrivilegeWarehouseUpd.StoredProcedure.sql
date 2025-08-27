SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_WMS_UserSetup_UserPrivilegeWarehouseUpd]
	@UserID varchar(25),
	@WarehouseCode varchar(25),
	@AllowAccess bit,
	@UpdateBy varchar(25)
as 

	insert into SS_UserWarehousePrivilege (UserID, WarehouseCode, AllowAccess, Last_User, Last_Update)
	values (@UserID, @WarehouseCode, @AllowAccess, @UpdateBy, getdate())


GO
