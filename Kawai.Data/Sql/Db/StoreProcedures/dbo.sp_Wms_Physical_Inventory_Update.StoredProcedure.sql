SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Physical_Inventory_Update]
(
	@LastUser	nvarchar(50),
	@Details	dbo.tvp_StockDetail READONLY
)
as
begin
	--VALIDASI AKSES WAREHOUSE
	if not exists (select 1 from SS_UserWarehousePrivilege where UserID = @LastUser and isnull(AllowAccess, 0) = 1)
	begin
		raiserror('User tidak memiliki hak akses ke warehouse ini!', 16, 1)
		return
	end

	--VALDASI AKSES AREA
	if not exists (select 1 from SS_UserAreaPrivilege where UserID = @LastUser and isnull(AllowAccess, 0) = 1)
	begin
		raiserror('User tidak memiliki hak akses ke area ini!', 16, 1)
		return
	end

	--UPDATE JIDA SUDAH ADA DATA STOCK OPANME
	update so
	set so.InventoryQty = ud.InventoryQty
		,so.Lastupdate	= getdate()
		,so.LastUser	= @LastUser
	from StockOpname so
	inner join @Details ud 
		on so.RefNo			= ud.RefNo
		and so.WarehouseCode = ud.WarehouseCode  
		and so.AreaCode		= ud.AreaCode
		and so.AddressCode	= ud.AddressCode
		and so.ItemCode		= ud.ItemCode
		and so.LotNo		= ud.LotNo
		and so.BarcodeNo	= ud.BarcodeNo

	--INSERT JIKA BELUM ADA DATA STOCK OPNAME
	insert into StockOpname
	  (		
		[BarcodeNo]
        ,[LotNo]
        ,[ItemCode]
        ,[RefNo]
        ,[WarehouseCode]
        ,[AreaCode]
        ,[AddressCode]
        ,[InventoryQty]
        ,[RegisterDate]
        ,[RegisterUser]
        ,[LastUpdate]
        ,[LastUser]
	)
	select
		ud.BarcodeNo,
		ud.LotNo,
		ud.ItemCode,
		ud.RefNo,
		ud.WarehouseCode,
		ud.AreaCode,
		ud.AddressCode,
		ud.InventoryQty,
		GETDATE(),
		@LastUser,
		GETDATE(),
		@LastUser
	from @Details ud
	where not exists (
		select 1
		from StockOpname so
		where so.RefNo		= ud.RefNo
		and so.WarehouseCode = ud.WarehouseCode
		and so.AreaCode		= ud.AreaCode
		and so.AddressCode	= ud.AddressCode
		and so.ItemCode		= ud.ItemCode
		and so.LotNo		= ud.LotNo
		and so.BarcodeNo	= ud.BarcodeNo		
	)
end
GO
