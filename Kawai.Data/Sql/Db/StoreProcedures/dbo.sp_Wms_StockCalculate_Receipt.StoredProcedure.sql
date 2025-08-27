SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_StockCalculate_Receipt]
	@Id bigint
as
begin
	declare 
		@WarehouseCode	varchar(25),
		@AreaCode		varchar(25),
		@AddressCode	varchar(25),
		@ItemCode		varchar(25),
		@BarcodeNo		varchar(50),
		@LotNo			varchar(100),
		@QtyTrans		numeric(18,9),
		@InventoryQty	numeric(18,9),
		@SublotNo		int,
		@SourceType		varchar(50),
		@SourceRef		varchar(50),
		@RefMutationId	bigint,
		@RegisterUser	varchar(50)

	select 
		@WarehouseCode	= WarehouseCode,
		@AreaCode		= AreaCode,
		@AddressCode	= AddressCode,
		@ItemCode		= ItemCode,
		@BarcodeNo		= BarcodeNo,
		@LotNo			= LotNo,
		@QtyTrans		= QtyTrans,
		@InventoryQty	= InventoryQty,
		@SublotNo		= SublotNo,
		@SourceType		= SourceType,
		@SourceRef		= SourceRef,
		@RefMutationId	= RefMutationId,
		@RegisterUser	= RegisterUser
	From StockMutation 
	where Id = @Id

	declare @isManual bit, @receiptDate date, @idDetailBarcode bigint
	select @isManual = IsManual, @receiptDate = ReceiptDate from PartReceiptHeader where Id = @SourceRef

	set @SublotNo = isnull((select max(SublotNo) from PartReceiptDetailBarcode where ReceiptDate = @receiptDate), 0) + 1
	select top 1 @idDetailBarcode = Id from PartReceiptDetailBarcode where ReceiptId = @SourceRef and BarcodeNo is null order by Id

	if isnull(@isManual, 0) = 1
	begin
		declare @NewBarcode varchar(100)
		EXEC dbo.GenerateNumerator @Prefix = @BarcodeNo, @LengthSequence = 4, @Result = @NewBarcode OUTPUT;

		declare @NewLot varchar(100) = (select top 1 LotNo from PartReceiptDetailBarcode where left(LotNo, 12) = @LotNo order by LotNo desc)
		if @NewLot is null
		begin
			EXEC dbo.GenerateNumerator @Prefix = @LotNo, @LengthSequence = 4, @Result = @NewLot OUTPUT;			
		end

		EXEC sp_Wms_Stock_UpSertStockDetail @WarehouseCode, @AreaCode, @AddressCode, @ItemCode, @NewBarcode, @NewLot, @QtyTrans, @InventoryQty, @SublotNo, @RegisterUser
		EXEC sp_Wms_Stock_UpSertStockMaster @receiptDate, @WarehouseCode, @AreaCode, @ItemCode, @NewLot, @QtyTrans, @InventoryQty, 'R', @RegisterUser
	end
	else 
	begin
		EXEC sp_Wms_Stock_UpSertStockDetail @WarehouseCode, @AreaCode, @AddressCode, @ItemCode, @BarcodeNo, @LotNo, @QtyTrans, @InventoryQty, @SublotNo, @RegisterUser
		EXEC sp_Wms_Stock_UpSertStockMaster @receiptDate, @WarehouseCode, @AreaCode, @ItemCode, @LotNo, @QtyTrans, @InventoryQty, 'R', @RegisterUser
	end

	update PartReceiptDetailBarcode set BarcodeNo = @NewBarcode, LotNo = @NewLot, SublotNo = @SublotNo where Id = @idDetailBarcode
	update StockMutation set BarcodeNo = @BarcodeNo, LotNo = @LotNo, SublotNo = @SublotNo, HasCalculate = 1, StatusCalculate = 'OK', CalculateDate = getdate() 
	where Id = @Id
end



GO
