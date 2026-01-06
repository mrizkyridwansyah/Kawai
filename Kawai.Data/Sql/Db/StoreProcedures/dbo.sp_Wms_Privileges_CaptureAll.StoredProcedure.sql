SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [sp_Wms_Privileges_CaptureAll]
	@UserID varchar(25)
as 

select 'Menu Privileges' Name, UserID, MenuID ObjectId, AllowAccess, AllowUpdate, AllowPrice From SS_UserPrivilege where UserID = @UserID
union all
select 'Warehouse Privileges' Name, UserID, WarehouseCode ObjectId, AllowAccess, null AllowUpdate, null AllowPrice From SS_UserWarehousePrivilege where UserID = @UserID

GO
