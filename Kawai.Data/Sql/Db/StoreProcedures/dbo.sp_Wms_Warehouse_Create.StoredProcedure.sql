SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [sp_Wms_Warehouse_Create]
	@FactoryCode varchar(25),
	@WarehouseCode varchar(25),
	@WarehouseName varchar(200),
	@AdmGroup varchar(15),
	@StockControlCls varchar(2),
	@NGCls varchar(2),
	@UseEndDate datetime = null,
	@RegisterBy varchar(25)
as
begin
	if exists (select 1 from WareHouse_Master where WH_Code = @WarehouseCode)
	begin
		raiserror('Warehouse Code Already Exists',16,1)
		return;
	end

	if @UseEndDate is null
	begin
		set @UseEndDate = cast('9999-12-31' as date)
	end

	insert into WareHouse_Master(WH_Code, WH_Name, Adm_Group, StockControl_Cls, NG_Cls, Use_EndDay, Last_User, Register_Date, Company_Code)
	values (@WarehouseCode, @WarehouseName, @AdmGroup, @StockControlCls, @NGCls, format(@UseEndDate, 'yyyyddMM'), @RegisterBy, getdate(), @FactoryCode)
end
GO
