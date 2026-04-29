
CREATE   procedure [sp_Wms_ReceiptUnschedule_Update]
	@Id				bigint,
	@DNNumber		varchar(50),
	@FactoryCode	varchar(25),
	@SupplierCode	varchar(25),
	@DNDate			date,
	@BCNumber		varchar(50),
	@BCType			varchar(15),
	@BCDate			date,
	@VehicleNo		varchar(15),
	@Transport		varchar(15),
	@ReferenceNo	varchar(50),
	@Details		tvp_ReceiptUnscheduleDetail READONLY,
	@UpdateBy		varchar(25)
as
begin
	if exists 
	(
		select * From @Details a
		left join ItemSupplierPacking b on a.ItemCode = b.ItemCode and b.SupplierCode = @SupplierCode
		left join Item_Master mi on a.ItemCode = mi.Item_Code
		where isnull(b.QtyPacking, mi.Number_Box) <= 0
	)
	begin
		raiserror('Qty Packing Item ini belum disetting!', 16, 1)
		return
	end

	if not exists (select 1 from PartReceiptHeader where Id = @Id)
	begin
		raiserror('Data Receipt tidak ditemukan!', 16, 1)
		return
	end

	if exists (select 1 from PartReceiptDetailBarcode where ReceiptId = @Id)
	begin
		raiserror('Data Receipt tidak bisa diubah karena sudah print label!', 16, 1)
		return
	end

	if not exists (select 1 from SS_UserFactoryPrivilege where UserID = @UpdateBy and isnull(AllowAccess, 0) = 1)
	begin
		raiserror('User tidak memiliki hak akses ke factory ini!', 16, 1)
		return
	end

	-- UPDATE DATA RECEIPT HEADER
	update PartReceiptHeader 
	set 
		CompanyCode = @FactoryCode,
		DNNumber	= @DNNumber, 
		DNDate		= @DNDate, 
		BCNumber	= @BCNumber, 
		BCType		= @BCType, 
		BCDate		= @BCDate, 
		VehicleNo	= @VehicleNo, 
		LastUpdate	= GETDATE(), 
		LastUser	= @UpdateBy, 
		Transport	= @Transport,
		ReferenceNo = @ReferenceNo
	where Id = @Id

	-- HAPUS DETAIL LAMA
	DELETE FROM PartReceiptDetail WHERE ReceiptId = @Id

	declare @ReceiptDate date = (select ReceiptDate From PartReceiptHeader where Id = @Id)

	-- INSERT DETAIL BARU
	insert into PartReceiptDetail (ReceiptId, ReceiptDate, PONumber, ItemCode, UnitCls, ExpectedQty, TotalPacking, ReceiptQty, Remarks)
	select @Id, @ReceiptDate, null, a.ItemCode, isnull(isp.UnitCls, mi.Unit_Cls), 0, CEILING(CAST(a.ReceiptQty AS FLOAT) / isnull(isp.QtyPacking, mi.Number_Box)), a.ReceiptQty, '' 
	from @Details a
	left join ItemSupplierPacking isp on a.ItemCode = isp.ItemCode and isp.SupplierCode = @SupplierCode
	left join Item_Master mi on a.ItemCode = mi.Item_Code

	-- HAPUS DETAIL LAMA DI PART RECEIPT EZR
	DELETE FROM Part_Receipt WHERE RefWMSReceiptId = @Id

	-- INSERT DETAIL BARU KE PART RECEIPT EZR
	declare @seqNo int = (isnull((select max(Seq_No) From Part_Receipt with (updlock, holdlock)), 0))
	DECLARE @BCTypeVal varchar(100) = (SELECT Description fROM BCType_Cls	WHERE BCType_Cls = @BCType)

	insert into Part_Receipt 
	(
		Seq_No, Supplier_Code, PO_No, Warehouse_Code, Address, Receipt_Cls, Receipt_Date, Item_Code, Qty, SerialNoFrom, SerialNoTo, 
		Unit_Cls, Currency_Code, Price, Amount, SuratJalan_No, ProductionResult_Cls, DailySeq_No, Remarks, Transport_Cls, Lot_No,
		Last_Update, Last_User, Register_Date, BC_Type, BC40_No, BC40_Date, Receipt_Status, No_Register, RefWMSReceiptId
	)
	select 
		@seqNo + ROW_NUMBER() OVER (ORDER BY dtl.Id), hd.SupplierCode, '', it.WH_Code, '' [Address], 'R', @ReceiptDate, dtl.ItemCode, dtl.ReceiptQty, null SerialNoFrom, null SerialNoTo,  
		dtl.UnitCls, pm.Currency_Code, pm.Price, pm.Price * dtl.ReceiptQty, hd.DNNumber, 0, null DailySeq_No, '', @Transport, NULL,
		getdate(), @UpdateBy, getdate(), isnull(@BCTypeVal, hd.BCType), hd.BCNumber, hd.BCDate, null Receipt_Status, hd.ReceiptNo, @Id
	From PartReceiptHeader hd
	inner join PartReceiptDetail dtl on hd.Id = dtl.ReceiptId
	left join Item_Master it on dtl.ItemCode = it.Item_Code
	left join Price_Master pm on dtl.ItemCode = pm.Item_Code and hd.SupplierCode = pm.Trade_Code and Price_Cls = '01'
	where hd.Id = @Id

end
