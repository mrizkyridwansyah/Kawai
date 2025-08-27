SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_WMS_UserSetup_UserPrivilegeWarehouse]
	@UserID varchar(25)
as 

select 
	a.WH_Code WarehouseCode, a.WH_Name WarehouseName, b.AllowAccess
From WareHouse_Master a
left join 
(
	select * From SS_UserWarehousePrivilege where UserID = @UserID
) b on a.WH_Code = b.WarehouseCode
GO
