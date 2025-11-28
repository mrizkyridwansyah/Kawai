SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Stock_MovingRef]
	@RefNo				varchar(50),
	@ToWarehouseCode	varchar(25),
	@ToAreaCode			varchar(25),
	@ToAddressCode		varchar(25),
	@ToRefNo			varchar(50),
	@UserId				varchar(25)
as
begin
	declare @tblStockDetail table 
	(
		RefNo			varchar(50) not null,
		WarehouseCode	varchar(25) not null,
		AreaCode		varchar(25) not null,
		AddressCode		varchar(25) not null,
		BarcodeNo		varchar(50) not null,
		ItemCode		varchar(25) not null,
		LotNo			varchar(100) not null,
		SublotNo		int null,
		Qty				numeric(18,9) null,
		InventoryQty	numeric(18,9) null,
		ExpiredDate		date null,
		ProductionDate	date null,
		ReceiptDate		date null,
		Supplier		varchar(150) null,
		PrintCls		bit null,
		DisposalCls		bit null,
		StatusReceipt	varchar(10) null,
		RegisterDate	datetime null,
		RegisterUser	varchar(25) null,
		Lastupdate		datetime null,
		LastUser		varchar(25)	null
	)
	insert into @tblStockDetail
	select 
		ISNULL(@ToRefNo, RefNo)			
		, @ToWarehouseCode	
		, @ToAreaCode		
		, @ToAddressCode		
		, BarcodeNo		
		, ItemCode		
		, LotNo			
		, SublotNo		
		, Qty				
		, InventoryQty	
		, ExpiredDate		
		, ProductionDate	
		, ReceiptDate		
		, Supplier		
		, PrintCls		
		, DisposalCls		
		, StatusReceipt	
		, RegisterDate	
		, RegisterUser	
		, Lastupdate		
		, LastUser		
	from StockDetail
	where RefNo = @RefNo
	and isnull(Qty, 0) > 0

	declare @tblStockHeader table
	(
		Urutan				int not null,
		RefNo				varchar(50) not null,
		WarehouseCode		varchar(25) not null,
		AreaCode			varchar(25) not null,
		ItemCode			varchar(25) not null,
		LotNo				varchar(100) not null,
		TotalQty			numeric(18,9) null,
		TotalInventoryQty	numeric(18,9) null
	)

	insert into @tblStockHeader
	select 
		ROW_NUMBER() over (order by LotNo), RefNo, WarehouseCode, AreaCode, ItemCode, LotNo, sum(Qty) TotalQtyDetail, sum(InventoryQty) TotalIvtQtyDetail 
	From StockDetail
	where RefNo = @RefNo
	and isnull(Qty, 0) > 0
	group by RefNo, WarehouseCode, AreaCode, ItemCode, LotNo

	declare @TransDate date = getdate()
	declare
		@WarehouseCode	varchar(25),
		@AreaCode		varchar(25),
		@ItemCode		varchar(25),
		@LotNo			varchar(100),
		@Qty			numeric(18,9),
		@InventoryQty	numeric(18,9)

	declare @i int = 1
	while @i <= (select count(1) from @tblStockHeader)
	begin
		select 
			@WarehouseCode = WarehouseCode, @AreaCode = AreaCode, @ItemCode = ItemCode, @LotNo = LotNo, 
			@Qty = TotalQty, @InventoryQty = null
		from @tblStockHeader where Urutan = @i

		exec sp_Wms_Stock_UpSertStockHeader @TransDate, @RefNo, @WarehouseCode, @AreaCode, @ItemCode, @LotNo, @Qty, @InventoryQty, 'S', @UserId

		set @i += 1
	end

	update StockDetail 
	set 
		Qty = 0, InventoryQty = case when InventoryQty is null then null else 0 end
	where RefNo = @RefNo
	and isnull(Qty, 0) > 0

	MERGE INTO StockDetail sd
	USING
	(
		SELECT * FROM @tblStockDetail
	) as sc
	(
		RefNo, WarehouseCode, AreaCode, AddressCode, BarcodeNo, ItemCode, LotNo, SublotNo, Qty, InventoryQty, ExpiredDate, ProductionDate, ReceiptDate, 
		Supplier, PrintCls, DisposalCls, StatusReceipt, RegisterDate, RegisterUser, Lastupdate, LastUser	
	)
	ON
	sd.RefNo = sc.RefNo 
	and sd.WarehouseCode	= sc.WarehouseCode 
	and sd.AreaCode			= sc.AreaCode
	and sd.AddressCode		= sc.AddressCode 
	and sd.BarcodeNo		= sc.BarcodeNo 
	and sd.ItemCode			= sc.ItemCode
	and sd.LotNo			= sc.LotNo
	WHEN MATCHED THEN
		UPDATE SET sd.Qty		= isnull(sc.Qty, 0),
				sd.InventoryQty	= sc.InventoryQty, 
				sd.LastUpdate	= GETDATE(),
				sd.LastUser		= @UserId
	WHEN NOT MATCHED THEN
		INSERT 
		(	
			RefNo			
			, WarehouseCode	
			, AreaCode		
			, AddressCode		
			, BarcodeNo		
			, ItemCode		
			, LotNo			
			, SublotNo		
			, Qty				
			, InventoryQty	
			, ExpiredDate		
			, ProductionDate	
			, ReceiptDate		
			, Supplier		
			, PrintCls		
			, DisposalCls		
			, StatusReceipt	
			, RegisterDate	
			, RegisterUser	
			, Lastupdate		
			, LastUser		
		)
		VALUES 
		(	
			sc.RefNo			
			, sc.WarehouseCode	
			, sc.AreaCode		
			, sc.AddressCode		
			, sc.BarcodeNo		
			, sc.ItemCode		
			, sc.LotNo			
			, sc.SublotNo		
			, sc.Qty				
			, sc.InventoryQty	
			, sc.ExpiredDate		
			, sc.ProductionDate	
			, sc.ReceiptDate		
			, sc.Supplier		
			, sc.PrintCls		
			, sc.DisposalCls		
			, sc.StatusReceipt	
			, sc.RegisterDate	
			, sc.RegisterUser	
			, sc.Lastupdate		
			, sc.LastUser		
		);

	delete from @tblStockHeader

	insert into @tblStockHeader
	select 
		ROW_NUMBER() over (order by LotNo), RefNo, WarehouseCode, AreaCode, ItemCode, LotNo, sum(Qty) TotalQtyDetail, sum(InventoryQty) TotalIvtQtyDetail 
	From StockDetail
	where RefNo = @RefNo
	and isnull(Qty, 0) > 0
	group by RefNo, WarehouseCode, AreaCode, ItemCode, LotNo

	set @i = 1
	while @i <= (select count(1) from @tblStockHeader)
	begin
		select 
			@WarehouseCode = WarehouseCode, @AreaCode = AreaCode, @ItemCode = ItemCode, @LotNo = LotNo, 
			@Qty = TotalQty, @InventoryQty = case when TotalInventoryQty is null then null else TotalInventoryQty end
		from @tblStockHeader where Urutan = @i

		exec sp_Wms_Stock_UpSertStockHeader @TransDate, @RefNo, @WarehouseCode, @AreaCode, @ItemCode, @LotNo, @Qty, @InventoryQty, 'R', @UserId

		set @i += 1
	end
end
GO
