SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_Warehouse_GetDetail]
	@WarehouseCode varchar(25)
as
begin
	select 
		wh.Company_Code FactoryCode, cp.Company_Name FactoryName, 
		wh.WH_Code WarehouseCode, wh.WH_Name WarehouseName, wh.Adm_Group AdmGroup, tm.Trade_Name AdmGroupName,
		wh.StockControl_Cls StockControlCls, wh.NG_Cls NGCls, 
		dbo.ConvertToDateTimeFromFuckingString(wh.Use_EndDay) UseEndDate, wh.Last_Update LastUpdate, us.FullName Lastuser 
	From WareHouse_Master wh
	left join vw_User us on wh.Last_User = us.UserID
	left join Trade_Master tm on wh.Adm_Group = tm.Trade_Code
	left join Company_Profile cp on wh.Company_Code = cp.Company_Code
	where wh.WH_Code = @WarehouseCode
end
GO
