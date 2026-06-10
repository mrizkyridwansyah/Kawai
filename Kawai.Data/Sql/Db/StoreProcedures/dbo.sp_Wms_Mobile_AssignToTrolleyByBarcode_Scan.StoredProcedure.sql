
CREATE   procedure [dbo].[sp_Wms_Mobile_AssignToTrolleyByBarcode_Scan]
	@BarcodeNo varchar(50),
	@TrolleyNo varchar(50),
	@UserId varchar(25)
as
begin
	declare @trolleyCls varchar(15), @trolleyDesc varchar(100)
	select @trolleyCls = Trolley_Cls, @trolleyDesc = Description From MS_Trolley where TrolleyCode = @TrolleyNo

	if @trolleyDesc is null
	begin
		raiserror('Data Trolley tidak ditemukan!', 16,1)
		return
	end

	if not exists (select 1 From PartMaterialRequestDetail where Trolley_No = @TrolleyNo)
	begin
		raiserror('Data Request untuk trolley ini tidak ditemukan!', 16,1)
		return
	end

	declare @pickingNo varchar(100), @requestDetailID bigint
	select top 1 @pickingNo = RefNumber, @requestDetailID = @requestDetailID 
	From PartMaterialRequestDetail where Trolley_No = @TrolleyNo order by RegisterDate desc

	if @pickingNo is null
	begin
		raiserror('Data picking tidak ditemukan!', 16,1)
		return
	end

	if not exists (select 1 from StockDetail where BarcodeNo = @BarcodeNo and Qty > 0)
	begin
		raiserror('Data Stock tidak ditemukan!', 16,1)
		return
	end

	declare 
		@refNo varchar(25), @warehouse varchar(25), @area varchar(25), @address varchar(25), @item varchar(25), @lotNo varchar(100), 
		@qty numeric(18,9), @ivtQty numeric(18,9), @pickingNoStock varchar(25), @statusReceipt varchar(50)

	select 
		@refNo = RefNo, @warehouse = WarehouseCode, @area = AreaCode, @address = AddressCode, @item = ItemCode, @lotNo = LotNo,
		@qty = Qty, @ivtQty = InventoryQty, @pickingNoStock = Picking_No, @statusReceipt = StatusReceipt 
	from StockDetail where BarcodeNo = @BarcodeNo and Qty > 0

	declare @warehouseTrolly varchar(25), @areaTrolly varchar(25), @addressTrolly varchar(25)

	select @warehouseTrolly = WarehouseCode, @areaTrolly = AreaCode, @addressTrolly = AddressCode
	from StockDetail where RefNo = @TrolleyNo and Qty > 0

	if exists (select 1 from Trade_Master where Trade_Cls = '3' and Subcon_WH_Code	= @warehouse)
	begin
		raiserror('Lokasi stock berada di warehouse subcon!', 16,1)
		return
	end
	
	if isnull(@pickingNoStock, '') <> isnull(@pickingNo, '')
	begin
		raiserror('Data stock sudah dialokasikan untuk produksi lain!', 16,1)
		return
	end

	if isnull(@statusReceipt, '') <> 'OK'
	begin
		raiserror('Status stock belum OK!', 16,1)
		return
	end

	declare @transDate date = getdate()

	exec sp_Wms_Stock_UpSertStockDetail @TrolleyNo, @warehouseTrolly, @areaTrolly, @addressTrolly, @item, @BarcodeNo, @lotNo, @qty, @ivtQty, null, @UserId, 'OK', ''
	exec sp_Wms_Stock_UpSertStockDetail @refNo, @warehouse, @area, @address, @item, @BarcodeNo, @lotNo, 0, 0, null, @UserId, 'OK', ''

	update StockDetail set Picking_No = @pickingNo where BarcodeNo = @BarcodeNo and Qty > 0

	if @refNo <> @TrolleyNo or @warehouse <> @warehouseTrolly or @area <> @areaTrolly
	begin
		exec sp_Wms_Stock_UpSertStockHeader @transDate, @refNo, @warehouse, @area, @item, @lotNo, @qty, null, 'S', @UserId
		EXEC sp_Wms_Stock_UpSertStockHeader @transDate, @TrolleyNo, @warehouseTrolly, @areaTrolly, @item, @lotNo, @qty, null, 'R', @UserId		
	end
end
