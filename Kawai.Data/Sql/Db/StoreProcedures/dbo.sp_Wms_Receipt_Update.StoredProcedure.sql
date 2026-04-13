SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   procedure [sp_Wms_Receipt_Update]
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
	@Remarks		varchar(max),
	@RegisterNo		varchar(max),
	@Details		tvp_ReceiptDetail READONLY,
	@UpdateBy		varchar(25)
as
begin
	if exists 
	(
		select 1 From @Details a
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
		raiserror('Data Receipt sudah tidak bisa diubah karena sudah print label!', 16, 1)
		return
	end

	if not exists (select 1 from SS_UserFactoryPrivilege where UserID = @UpdateBy and isnull(AllowAccess, 0) = 1)
	begin
		raiserror('User tidak memiliki hak akses ke factory ini!', 16, 1)
		return
	end

	declare @warehouseSubcon varchar(25), @tradeCls varchar(3) 
	select @warehouseSubcon = Subcon_WH_Code, @TradeCls = Trade_Cls from Trade_Master where Trade_Code = @SupplierCode

	if @tradeCls = '3' and isnull(@warehouseSubcon, '') = ''
	begin
		raiserror('Warehouse Subcon belum disetting di Trade Master!', 16, 1)
		return
	end

	declare @overReceiptPO varchar(max) = 
	(
		select STRING_AGG(a.PO_No, ', ') From PurchaseOrder_Detail a
		inner join Item_Master mi on a.Item_Code = mi.Item_Code
		left join 
		(
			select 
				prd.PONumber, prd.ItemCode, sum(prd.ReceiptQty) ReceiptQty 
			from PartReceiptDetail prd
			inner join @Details dl on prd.PONumber = dl.PONumber and prd.ItemCode = dl.ItemCode
			where prd.ReceiptId <> isnull(@Id, 0)
			group by prd.PONumber, prd.ItemCode
		) rcpSum on a.PO_No = rcpSum.PONumber and a.Item_Code = rcpSum.ItemCode
		inner join @Details dtl on a.PO_No = dtl.PONumber and a.Item_Code = dtl.ItemCode
		where a.Qty - (isnull(rcpSum.ReceiptQty, 0) + dtl.ReceiptQty) < 0
	)

	if isnull(@overReceiptPO, '') <> ''
	begin
		declare @errors varchar(max) = 'Over Qty ('+@overReceiptPO+')!'
		raiserror(@errors, 16, 1)
		return
	end

	if @tradeCls = '3'
	begin
		declare @tblRequirementMaterial table (PONumber varchar(100), ParentItem varchar(25), ItemCode varchar(25), BOMQty numeric(18,9), ReceiptQty numeric(18,9))
		insert into @tblRequirementMaterial
		select dtl.PONumber, dtl.ItemCode, bom.Item_Code, bom.Qty, dtl.ReceiptQty
		From BOM_Master bom 
		inner join @Details dtl on bom.Parent_ItemCode = dtl.ItemCode

		declare @pickingNos table (PONumber varchar(100), ParentItem varchar(25), PickingNo varchar(100))
		insert into @pickingNos
		select distinct hd.PO_NO, hd.ParentItem_Code, dtl.RefNumber 
		From PartMaterialRequestDetail_PO dtl
		inner join PartMaterialRequestHeader_PO hd on dtl.RequestID = hd.RequestID
		inner join PartMaterialRequestItemDetail_PO dtli on dtl.RequestDetailID = dtli.RequestDetailID
		inner join @Details req on hd.PO_NO = req.PONumber

		declare @tblPOItemSummaryBOM table (PONumber varchar(100), ParentItem varchar(25), QtyReceipt numeric(18,9), QtyMinCanReceipt numeric(18,9))
		insert into @tblPOItemSummaryBOM
		select res.PONumber, res.ParentItem, res.QtyReceipt, min(QtyCanReceipt) from 
		(
			select 
				req.PONumber, req.ParentItem, 
				QtyReceipt = req.ReceiptQty,
				QtyCanReceipt = floor(isnull(stok.OutstandingQty, 0) / req.BOMQty)
			from @tblRequirementMaterial req
			left join
			(
				select x.PONumber, x.ParentItem, ItemCode, sum(Qty) OutstandingQty 
				From StockDetail std
				inner join @pickingNos x on isnull(std.Picking_No, '') = x.PickingNo
				where 1=1
				and WarehouseCode = @warehouseSubcon 
				and Qty > 0 
				group by x.PONumber, x.ParentItem, ItemCode
			) stok on req.PONumber = stok.PONumber and req.ParentItem = stok.ParentItem and stok.ItemCode = req.ItemCode
		) res
		group by res.PONumber, res.ParentItem, res.QtyReceipt

		declare @msg varchar(max)
		if exists (select 1 from @tblPOItemSummaryBOM where QtyMinCanReceipt < QtyReceipt)
		begin
			SELECT @msg = STRING_AGG(
				PONumber + ' (Need: ' + CAST(QtyReceipt AS VARCHAR) +
				', Can: ' + CAST(QtyMinCanReceipt AS VARCHAR) + ')'
			, '; ')
			FROM @tblPOItemSummaryBOM
			WHERE QtyMinCanReceipt < QtyReceipt
			RAISERROR('Material di warehouse subcon tidak mencukupi untuk PO: %s', 16, 1, @msg)
			return	
		end
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
		Remarks		= @Remarks,
		RegisterNo	= @RegisterNo
	where Id = @Id

	-- HAPUS DETAIL LAMA
	DELETE FROM PartReceiptDetail WHERE ReceiptId = @Id

	declare @ReceiptDate date = (select ReceiptDate From PartReceiptHeader where Id = @Id)

	-- INSERT DETAIL BARU
	insert into PartReceiptDetail (ReceiptId, ReceiptDate, PONumber, ItemCode, UnitCls, ExpectedQty, TotalPacking, ReceiptQty, Remarks)
	select 
		@Id, @ReceiptDate, a.PONumber, a.ItemCode, a.UnitClsCode, a.ExpectedQty, CEILING(CAST(a.ReceiptQty AS FLOAT) / isnull(isp.QtyPacking, mi.Number_Box)), a.ReceiptQty, @Remarks
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
		@seqNo + ROW_NUMBER() OVER (ORDER BY dtl.Id), hd.SupplierCode, dtl.PONumber, it.WH_Code, '' [Address], 'R', @ReceiptDate, dtl.ItemCode, dtl.ReceiptQty, null SerialNoFrom, null SerialNoTo,  
		dtl.UnitCls, pod.Currency_Code, pod.Price, pod.Price * dtl.ReceiptQty, hd.DNNumber, 0, null DailySeq_No, @Remarks, @Transport, NULL,
		getdate(), @UpdateBy, getdate(), isnull(@BCTypeVal, hd.BCType), hd.BCNumber, hd.BCDate, null Receipt_Status, @RegisterNo, @Id
	From PartReceiptHeader hd
	inner join PartReceiptDetail dtl on hd.Id = dtl.ReceiptId
	left join Item_Master it on dtl.ItemCode = it.Item_Code
	left join PurchaseOrder_Detail pod on dtl.ItemCode = pod.Item_Code and dtl.PONumber = pod.PO_No
	where hd.Id = @Id
end
GO
