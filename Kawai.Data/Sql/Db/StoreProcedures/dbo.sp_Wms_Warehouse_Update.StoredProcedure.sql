SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [sp_Wms_Warehouse_Update]
	@FactoryCode varchar(25),
	@WarehouseCode varchar(25),
	@WarehouseName varchar(200),
	@AdmGroup varchar(15),
	@StockControlCls varchar(2),
	@NGCls varchar(2),
	@UseEndDate datetime = null,
	@UpdateBy varchar(25)
as
begin
	if not exists (select 1 from WareHouse_Master where WH_Code = @WarehouseCode)
	begin
		raiserror('Warehouse Code didn''t Exists',16,1)
		return;
	end

	if @UseEndDate is null
	begin
		set @UseEndDate = cast('9999-12-31' as date)
	end

	update WareHouse_Master 
	set 
		WH_Name = @WarehouseName, 
		Adm_Group = @AdmGroup, 
		StockControl_Cls = @StockControlCls, 
		NG_Cls = @NGCls, 
		Use_EndDay = format(@UseEndDate, 'yyyyMMdd'), 
		Last_User = @UpdateBy, 
		Last_Update = getdate() 
	where WH_Code = @WarehouseCode
end
GO
