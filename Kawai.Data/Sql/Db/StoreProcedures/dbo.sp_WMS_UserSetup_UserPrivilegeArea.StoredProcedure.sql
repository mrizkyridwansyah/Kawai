SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE procedure [sp_WMS_UserSetup_UserPrivilegeArea]
	@UserID varchar(25)
as 

select 
	a.AreaCode, a.AreaName, b.AllowAccess, a.WarehouseCode, wh.Company_Code FactoryCode, wh.WH_Name WarehouseName
From MS_Area a
inner join WareHouse_Master wh on a.WarehouseCode = wh.WH_Code
left join 
(
	select * From SS_UserAreaPrivilege where UserID = @UserID
) b on a.AreaCode = b.AreaCode
GO
