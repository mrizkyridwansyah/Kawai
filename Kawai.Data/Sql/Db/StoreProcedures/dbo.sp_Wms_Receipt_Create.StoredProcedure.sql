SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_Receipt_Create]
	@ReceiptNo		varchar(50),
	@IsManual		bit,
	@DNNumber		varchar(50),
	@SupplierCode	varchar(25),
	@DNDate			date,
	@BCNumber		varchar(50),
	@BCType			varchar(15),
	@BCDate			date,
	@VehicleNo		varchar(15),
	@Details		tvp_ReceiptDetail READONLY,
	@RegisterBy		varchar(25)
as
begin
	if @IsManual = 1 
	and exists 
	(
		select * From @Details a
		left join ItemSupplierPacking b on a.ItemCode = b.ItemCode and b.SupplierCode = @SupplierCode
		where b.QtyPacking is null
	)
	begin
		raiserror('Qty Packing Item Supplier ini belum disetting!', 16, 1)
		return
	end

	begin try
		begin transaction ReceiptTransaction

		declare @ReceiptDate date = getdate()

		insert into PartReceiptHeader (ReceiptNo, ReceiptDate, SupplierCode, DNNumber, DNDate, BCNumber, BCType, BCDate, VehicleNo, RegisterDate, RegisterUser, IsManual)
		values (@ReceiptNo, @ReceiptDate, @SupplierCode, @DNNumber, @DNDate, @BCNumber, @BCType, @BCDate, @VehicleNo, getdate(), @RegisterBy, @IsManual)

		declare @newid bigint = (select SCOPE_IDENTITY())

		insert into PartReceiptDetail (ReceiptId, ReceiptDate, PONumber, ItemCode, UnitCls, ExpectedQty, TotalPacking, ReceiptQty, IQCResult, HasValid)
		select @newid, @ReceiptDate, PONumber, ItemCode, UnitClsCode, ExpectedQty, TotalPacking, ReceiptQty, IQCResult, 1 from @Details

		declare @dt varchar(8) = format(getdate(), 'yyyyMMdd')
		declare @prefixBarcode varchar(10) = 'RM' + @dt
		declare @prefixLot varchar(12) = 'L.RM' + @dt

		declare @NewLot varchar(100) = (select top 1 LotNo from PartReceiptDetailBarcode where left(LotNo, 12) = @prefixLot order by LotNo desc)
		if @NewLot is null
		begin
			EXEC dbo.GenerateNumerator @Prefix = @prefixLot, @LengthSequence = 4, @Result = @NewLot OUTPUT;			
		end

		declare @i int = 1

		if @IsManual = 1
		begin
			declare @partReceiptDetail table 
			(
				[Urutan] int,
				[PONumber] [varchar](50) ,
				[ItemCode] [varchar](25) ,
				[WarehouseCode] [varchar](25) ,
				[TotalPacking] int,
				[QtyPacking] [numeric](18, 9)
			)
	
			insert into @partReceiptDetail
			select ROW_NUMBER() over (order by Id), PONumber, a.ItemCode, c.WH_Code, TotalPacking, b.QtyPacking
			From PartReceiptDetail a
			inner join ItemSupplierPacking b on a.ItemCode = b.ItemCode and b.SupplierCode = @SupplierCode
			inner join Item_Master c on a.ItemCode = c.Item_Code
			where ReceiptId = @newid

			while @i <= (select count(1) from @partReceiptDetail)
			begin
				declare @PONumber varchar(50), @ItemCode varchar(25), @WarehouseCode varchar(25), @TotalPacking int, @QtyPacking numeric(18,9)
				select @PONumber = PONumber, @ItemCode = ItemCode, @WarehouseCode = WarehouseCode, @TotalPacking = TotalPacking, @QtyPacking = QtyPacking 
				From @partReceiptDetail where Urutan = @i

				declare @j int = 1
				while @j <= @TotalPacking
				begin
					declare @NewBarcode varchar(100)
					EXEC dbo.GenerateNumerator @Prefix = @prefixLot, @LengthSequence = 4, @Result = @NewBarcode OUTPUT;

					declare @SublotNo int = isnull((select max(SublotNo) from PartReceiptDetailBarcode where ReceiptDate = getdate()), 0) + 1

					insert into PartReceiptDetailBarcode (ReceiptId, ReceiptDate, PONumber,ItemCode, BarcodeNo, LotNo, SublotNo, Qty)
					values (@newid, getdate(), @PONumber, @ItemCode, @NewBarcode, @NewLot, @SublotNo, @QtyPacking)

					EXEC sp_Wms_Stock_UpSertStockDetail @WarehouseCode, 'TMP', 'TMP', @ItemCode, @NewBarcode, @NewLot, @QtyPacking, NULL, @SublotNo, @RegisterBy
					EXEC sp_Wms_Stock_UpSertStockMaster @receiptDate, @WarehouseCode, 'TMP', @ItemCode, @NewLot, @QtyPacking, NULL, 'R', @RegisterBy

					set @j += 1
				end

				set @i += 1
			end
		end

		commit transaction ReceiptTransaction

		select @newid
	end try
	begin catch
		rollback transaction ReceiptTransaction
		declare @msg varchar(max) = (select ERROR_MESSAGE())
		raiserror(@msg,16,1)
		return
	end catch

end
GO
