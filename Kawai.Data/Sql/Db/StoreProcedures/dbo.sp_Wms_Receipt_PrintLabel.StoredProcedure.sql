SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [sp_Wms_Receipt_PrintLabel]
	@ReceiptId		varchar(50),
	@MustPrint	bit = null,
	@UserId			varchar(25)
as
begin
	if not exists (select 1 from PartReceiptHeader where Id = @ReceiptId)
	begin
		raiserror('Data Receipt tidak ditemukan!', 16, 1)
		return
	end

	if not exists 
	(
		select * From 
		(
			select * from PartReceiptHeader where Id = @ReceiptId
		) a 
		inner join 
		(
			select * from SS_UserFactoryPrivilege where UserID = @UserId and isnull(AllowAccess, 0) = 1
		) b on a.CompanyCode = b.FactoryCode
	)
	begin
		raiserror('User tidak memiliki hak akses ke factory ini!', 16, 1)
		return
	end

	if exists (select 1 from PartReceiptDetailBarcode where ReceiptId = @ReceiptId)
	begin
		if isnull(@MustPrint, 0) = 1
		begin
			update PartReceiptDetailBarcode set PrintStatus = null, PrintDate = null, PrintUser = @UserId 
			where ReceiptId = @ReceiptId
		end
	end
	else
	begin
		declare @ReceiptDate date, @SupplierCode varchar(50) 
		select @ReceiptDate = ReceiptDate, @SupplierCode = SupplierCode from PartReceiptHeader where Id = @ReceiptId

		declare @dt varchar(8) = format(@ReceiptDate, 'yyyyMMdd')
		declare @prefixBarcode varchar(10) = 'RM' + @dt
		declare @prefixLot varchar(12) = 'L.RM' + @dt

		declare @NewLot varchar(100)
		EXEC dbo.GenerateNumerator @Prefix = @prefixLot, @LengthSequence = 4, @Result = @NewLot OUTPUT;			

		declare @i int = 1
		declare @partReceiptDetail table 
		(
			[Urutan] int,
			[Id] bigint,
			[PONumber] [varchar](50) ,
			[ItemCode] [varchar](25) ,
			[ReceiptQty] [numeric](18,9),
			[QtyPacking] [numeric](18, 9),
			[WarehouseCode] [varchar](25)
		)
	
		insert into @partReceiptDetail
		select ROW_NUMBER() over (order by Id), Id, a.PONumber, a.ItemCode, a.ReceiptQty, isnull(b.QtyPacking, mi.Number_Box), isnull(po.WHTo, mi.WH_Code)
		From PartReceiptDetail a
		left join ItemSupplierPacking b on a.ItemCode = b.ItemCode and b.SupplierCode = @SupplierCode
		left join PurchaseOrder_Master po on a.PONumber = po.PO_No
		inner join Item_Master mi on a.ItemCode = mi.Item_Code
		where ReceiptId = @ReceiptId

		while @i <= (select count(1) from @partReceiptDetail)
		begin
			declare 
				@ReceiptDetailId bigint, @PONumber varchar(50), @ItemCode varchar(25), 
				@ReceiptQty numeric(18,9), @QtyPacking numeric(18,9), @WarehouseCode varchar(25)

			select 
				@ReceiptDetailId = Id, @PONumber = PONumber, @ItemCode = ItemCode,
				@ReceiptQty = ReceiptQty, @QtyPacking = QtyPacking, @WarehouseCode = WarehouseCode
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

				if isnull(@MustPrint, 0) = 1
				begin
					insert into PartReceiptDetailBarcode (ReceiptDetailId, ReceiptId, ReceiptDate, PONumber,ItemCode, BarcodeNo, LotNo, SublotNo, Qty, WarehouseCode)
					values (@ReceiptDetailId, @ReceiptId, @ReceiptDate, isnull(@PONumber, ''), @ItemCode, @NewBarcode, @NewLot, @SublotNo, @tempQty, @WarehouseCode)
				end
				else
				begin
					insert into PartReceiptDetailBarcode 
					(ReceiptDetailId, ReceiptId, ReceiptDate, PONumber,ItemCode, BarcodeNo, LotNo, SublotNo, Qty, WarehouseCode, PrintDate, PrintStatus, PrintUser)
					values 
					(@ReceiptDetailId, @ReceiptId, @ReceiptDate, isnull(@PONumber, ''), @ItemCode, @NewBarcode, @NewLot, @SublotNo, @tempQty, @WarehouseCode, getdate(), 1, @UserId)
				end

				set @ReceiptQty -= @tempQty
			end

			set @i += 1
		end

		UPDATE Part_Receipt SET Lot_No = @NewLot WHERE RefWMSReceiptId = @ReceiptId
	end
end
GO
