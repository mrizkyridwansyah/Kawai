SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   procedure [sp_Wms_ReceiptUnschedule_Create]
	@ReceiptNo		varchar(50),
	@DNNumber		varchar(50),
	@SupplierCode	varchar(25),
	@DNDate			date,
	@BCNumber		varchar(50),
	@BCType			varchar(15),
	@BCDate			date,
	@VehicleNo		varchar(15),
	@Transport		varchar(15),
	@Details		tvp_ReceiptUnscheduleDetail READONLY,
	@RegisterBy		varchar(25)
as
begin
	if exists 
	(
		select * From @Details a
		left join ItemSupplierPacking b on a.ItemCode = b.ItemCode and b.SupplierCode = @SupplierCode
		where b.QtyPacking is null
	)
	begin
		raiserror('Qty Packing Item Supplier ini belum disetting!', 16, 1)
		return
	end

	declare @ReceiptDate date = getdate()

	begin transaction receiptTransaction
	begin try
		insert into PartReceiptHeader 
		(ReceiptNo, ReceiptDate, SupplierCode, DNNumber, DNDate, BCNumber, BCType, BCDate, VehicleNo, RegisterDate, RegisterUser, IsManual, Transport, Remarks, SourceMenu)
		values 
		(@ReceiptNo, @ReceiptDate, @SupplierCode, @DNNumber, @DNDate, @BCNumber, @BCType, @BCDate, @VehicleNo, getdate(), @RegisterBy, 1, @Transport, '', 'RECEIPT UNSCHEDULE')

		declare @newid bigint = (select SCOPE_IDENTITY())

		insert into PartReceiptDetail (ReceiptId, ReceiptDate, PONumber, ItemCode, UnitCls, ExpectedQty, TotalPacking, ReceiptQty, Remarks)
		select @newid, @ReceiptDate, NULL, a.ItemCode, mi.UnitCls, 0, CEILING(CAST(a.ReceiptQty AS FLOAT) / mi.QtyPacking), a.ReceiptQty, '' 
		from @Details a
		inner join ItemSupplierPacking mi on a.ItemCode = mi.ItemCode

		declare @seqNo int = (isnull((select max(Seq_No) From Part_Receipt with (updlock, holdlock)), 0))

		insert into Part_Receipt 
		(
			Seq_No, Supplier_Code, PO_No, Warehouse_Code, Address, Receipt_Cls, Receipt_Date, Item_Code, Qty, SerialNoFrom, SerialNoTo, 
			Unit_Cls, Currency_Code, Price, Amount, SuratJalan_No, ProductionResult_Cls, DailySeq_No, Remarks, Transport_Cls,
			Last_Update, Last_User, Register_Date, BC_Type, BC40_No, BC40_Date, Receipt_Status, No_Register, RefWMSReceiptId
		)
		select 
			@seqNo + ROW_NUMBER() OVER (ORDER BY dtl.Id), hd.SupplierCode, '' , it.WH_Code, '' [Address], 'R', @ReceiptDate, dtl.ItemCode, dtl.ReceiptQty, null SerialNoFrom, null SerialNoTo,  
			dtl.UnitCls, null Currency, null Price, null Amount, hd.DNNumber, 0, null DailySeq_No, '', @Transport,
			getdate(), @RegisterBy, getdate(), hd.BCType, hd.BCNumber, hd.BCDate, null Receipt_Status, hd.ReceiptNo, hd.Id
		From PartReceiptHeader hd
		inner join PartReceiptDetail dtl on hd.Id = dtl.ReceiptId
		left join Item_Master it on dtl.ItemCode = it.Item_Code
		where hd.Id = @newid

		declare @dt varchar(8) = format(@ReceiptDate, 'yyyyMMdd')
		declare @prefixBarcode varchar(10) = 'RM' + @dt
		declare @prefixLot varchar(12) = 'L.RM' + @dt

		declare @NewLot varchar(100) = (select top 1 LotNo from PartReceiptDetailBarcode where left(LotNo, 12) = @prefixLot order by LotNo desc)
		if @NewLot is null
		begin
			EXEC dbo.GenerateNumerator @Prefix = @prefixLot, @LengthSequence = 4, @Result = @NewLot OUTPUT;			
		end

		declare @i int = 1
		declare @partReceiptDetail table 
		(
			[Urutan] int,
			[Id] bigint,
			[ItemCode] [varchar](25) ,
			[WarehouseCode] [varchar](25) ,
			[ReceiptQty] [numeric](18,9),
			[QtyPacking] [numeric](18, 9)
		)
	
		insert into @partReceiptDetail
		select ROW_NUMBER() over (order by Id), Id, a.ItemCode, c.WH_Code, a.ReceiptQty, b.QtyPacking
		From PartReceiptDetail a
		inner join ItemSupplierPacking b on a.ItemCode = b.ItemCode and b.SupplierCode = @SupplierCode
		inner join Item_Master c on a.ItemCode = c.Item_Code
		where ReceiptId = @newid

		while @i <= (select count(1) from @partReceiptDetail)
		begin
			declare 
				@ReceiptDetailId bigint, @ItemCode varchar(25), @WarehouseCode varchar(25), 
				@ReceiptQty numeric(18,9), @QtyPacking numeric(18,9)

			select 
				@ReceiptDetailId = Id, @ItemCode = ItemCode, @WarehouseCode = WarehouseCode, 
				@ReceiptQty = ReceiptQty, @QtyPacking = QtyPacking 
			From @partReceiptDetail where Urutan = @i

			while @ReceiptQty > 0
			begin
				declare @tempQty numeric(18,9) = @QtyPacking

				declare @NewBarcode varchar(100) 
				EXEC dbo.GenerateNumerator @Prefix = @prefixBarcode, @LengthSequence = 4, @Result = @NewBarcode OUTPUT;

				declare @SublotNo int = isnull((select max(SublotNo) from PartReceiptDetailBarcode where ReceiptDate = @ReceiptDate), 0) + 1

				if @ReceiptQty < @QtyPacking
				begin
					set @tempQty = @ReceiptQty
				end

				insert into PartReceiptDetailBarcode (ReceiptDetailId, ReceiptId, ReceiptDate, PONumber,ItemCode, BarcodeNo, LotNo, SublotNo, Qty)
				values (@ReceiptDetailId, @newid, @ReceiptDate, '', @ItemCode, @NewBarcode, @NewLot, @SublotNo, @tempQty)

				set @ReceiptQty -= @tempQty
			end

			set @i += 1
		end

		UPDATE Part_Receipt SET Lot_No = @NewLot WHERE RefWMSReceiptId = @newid

		select @newid

		commit transaction receiptTransaction
	end try
	begin catch
		rollback transaction receiptTransaction
		declare @msg varchar(max) = (select ERROR_MESSAGE())
		raiserror(@msg, 16, 1)
		return
	end catch

end
GO
