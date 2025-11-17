SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [vw_WarehouseLine]
as
select res.*, cc.Company_Name FactoryName From 
(
	select RTRIM(WH_Code) WarehouseCode, RTRIM(WH_Name) WarehouseName, Company_Code FactoryCode From WareHouse_Master
	union all
	select RTRIM(Line_Code) WarehouseCode, RTRIM(Line_Name) WarehouseName, Company_Code FactoryCode From Manufacture_Line
) res
left join Company_Profile cc on res.FactoryCode = cc.Company_Code
GO
