SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [sp_Wms_Warehouse_Import]
	@DataImport tvp_WarehouseImport READONLY,
	@UserId varchar(25)
as
begin	
	insert into WareHouse_Master (WH_Code, WH_Name, Adm_Group, StockControl_Cls, Use_EndDay, NG_Cls, Company_Code, Register_Date, Last_User)
	select WarehouseCode, WarehouseName, AdmGroup, StockControlCls, format(UseEndDate, 'yyyyMMdd'), NGCls, FactoryCode, getdate(), @UserId 
	From @DataImport
end
GO
