SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Mobile_PhysicalInventory_Save]
	@AddressCode varchar(25),
	@BarcodeNo varchar(50),
	@InventoryQty numeric(18,9),
	@UserId varchar(25)
as
begin
	-- ambil data warehouse & area dari address tsb sebagai default
	declare @warehouseCode varchar(25), @areaCode varchar(25), @areaName varchar(100), @allowed bit
	select @warehouseCode = b.WarehouseCode, @areaCode = a.AreaCode, @areaName = b.AreaName, @allowed = isnull(priv.AllowAccess, 0)
	from MS_Address a 
	inner join MS_Area b on a.AreaCode = b.AreaCode 
	left join 
	(
		select * from SS_UserAreaPrivilege where UserID = @UserId and AllowAccess = 1
	) priv on a.AreaCode = priv.AreaCode
	where AddressCode = @AddressCode

	declare @errMsg varchar(max) = ''

	if isnull(@allowed, 0) = 0
	begin
		set @errMsg = 'User tidak memiliki hak akses ke area ' + isnull(@areaName, '')
		raiserror(@errMsg, 16, 1)
		return
	end

	if not exists (select 1 From StockDetail where BarcodeNo = @BarcodeNo and AddressCode = @AddressCode and Qty > 0)
	begin
		set @errMsg = 'Data stock tidak ditemukan di area ini'
		raiserror(@errMsg, 16, 1)
		return
	end

	-- ambil warehouse & area dari stock
	DECLARE @warehouse varchar(25), @area varchar(25), @refNo varchar(50)
	select @warehouse = WarehouseCode, @area = AreaCode, @refNo = RefNo
	From StockDetail 
	where BarcodeNo = @BarcodeNo and AddressCode = @AddressCode and Qty > 0

	/*
		untuk SO ke table terpisah, dengan PK BarcodeNo aja. 
		jadi di StockDetail ga perlu update2 kolom InventoryQty & di StokHeader ga perlu update2 kolom TMInventory. 
		nanti pas CLOSING, baru disinkronisasikan aja dgn StockDetail & kalkulasi untuk TMInventory StockHeader. 
	*/

	if exists (select 1 from StockOpname where BarcodeNo = @BarcodeNo)
	begin
		-- set warehouse & area dari data stock dulu, kalo null maka ambil dari master address nya.
		update StockOpname
		set 
			RefNo = isnull(@refNo, ''), 
			WarehouseCode = isnull(@warehouse, @warehouseCode), 
			AreaCode = isnull(@area, @areaCode), 
			AddressCode = @AddressCode,
			InventoryQty = @InventoryQty,
			LastUpdate = getdate(),
			LastUser = @UserId
		where BarcodeNo = @BarcodeNo
	end
	else 
	begin
		insert into StockOpname(BarcodeNo, LotNo, ItemCode, RefNo, WarehouseCode, AreaCode, AddressCode, InventoryQty, RegisterDate, RegisterUser)
		select BarcodeNo, LotNo, ItemCode, isnull(@refNo, ''), isnull(@warehouse, @warehouseCode), isnull(@area, @areaCode), @AddressCode, @InventoryQty, getdate(), @UserId 
		From StockDetail 
		where BarcodeNo = @BarcodeNo and AddressCode = @AddressCode and Qty > 0
	end
end
GO
