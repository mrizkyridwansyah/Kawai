SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE procedure [sp_WMS_UserSetup_UserPrivilegeWarehouse]
	@UserID varchar(25)
as 

select 
	a.WH_Code WarehouseCode, a.WH_Name WarehouseName, b.AllowAccess, a.Company_Code FactoryCode, cp.Company_Name FactoryName
From WareHouse_Master a
left join Company_Profile cp on a.Company_Code = cp.Company_Code
left join 
(
	select * From SS_UserWarehousePrivilege where UserID = @UserID
) b on a.WH_Code = b.WarehouseCode
union all
select 
	a.Line_Code WarehouseCode, a.Line_Name WarehouseName, b.AllowAccess, a.Company_Code FactoryCode, cp.Company_Name FactoryName
From 
(
	select distinct Line_Code, Line_Name, Company_Code from Manufacture_Line
) a
left join Company_Profile cp on a.Company_Code = cp.Company_Code
left join 
(
	select * From SS_UserWarehousePrivilege where UserID = @UserID
) b on a.Line_Code = b.WarehouseCode

GO
