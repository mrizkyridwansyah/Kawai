SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [sp_Wms_Mobile_PhysicalInventory_Save]
	@AddressCode varchar(25),
	@BarcodeNo varchar(50),
	@InventoryQty numeric(18,9),
	@UserId varchar(25)
as
begin
	declare @areaCode varchar(25), @areaName varchar(100), @allowed bit
	select @areaCode = a.AreaCode, @areaName = b.AreaName, @allowed = isnull(priv.AllowAccess, 0)
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

	if exists (select 1 from StockOpname where BarcodeNo = @BarcodeNo)
	begin
		update StockOpname
		set 
			InventoryQty = @InventoryQty,
			LastUpdate = getdate(),
			LastUser = @UserId
		where BarcodeNo = @BarcodeNo
	end
	else 
	begin
		insert into StockOpname(BarcodeNo, LotNo, ItemCode, InventoryQty, RegisterDate, RegisterUser)
		select BarcodeNo, LotNo, ItemCode, @InventoryQty, getdate(), @UserId 
		From StockDetail where BarcodeNo = @BarcodeNo and AddressCode = @AddressCode and Qty > 0
	end
end
GO
