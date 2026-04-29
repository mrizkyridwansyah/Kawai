CREATE procedure [dbo].[sp_Wms_IQCResult_ApprovalSA]
	@InspectionId bigint,
	@InspectionResult varchar(100),
	@RemarksSA varchar(max),
	@UserId varchar(25)
as
begin
	if exists (select 1 from IQC_Inspection_Header where InspectionID = @InspectionId and InspectionResultSADate is not null)
	begin
		raiserror('Data QC sudah diapprove', 16,1)
		return
	end

	if exists 
	(
		select 1 From IQC_Inspection_Header iqc
		inner join PartReceiptHeader prh on iqc.ReceiptNo = prh.ReceiptNo
		inner join PartReceiptDetailBarcode prdb on prh.Id = prdb.ReceiptId and iqc.ItemCode = prdb.ItemCode
		where iqc.InspectionID = @InspectionId and isnull(IsVerified, 0) = 0
	) 
	begin
		raiserror('Silahkan receive semua barcode terlebih dahulu!', 16,1)
		return
	end

	declare @Source varchar(50), @ReceiptId bigint, @SupplierCode varchar(25), @DNNumber varchar(50), @PONumber varchar(50), @ItemCode varchar(25), @totalNGQty numeric(18,9), @remarks varchar(max), @receiptDate date, @factoryCode varchar(25)
	select 
		@Source = Soruce, @ReceiptId = prh.Id, @ItemCode = iqch.ItemCode, @totalNGQty = isnull(TotalQtyNG, 0), @remarks = iqch.Remarks, @receiptDate = prh.ReceiptDate, @factoryCode = prh.CompanyCode, @DNNumber = prh.DNNumber, @PONumber = iqch.PO_Number, @SupplierCode = prh.SupplierCode
	from IQC_Inspection_Header iqch
	inner join PartReceiptHeader prh on iqch.ReceiptNo = prh.ReceiptNo
	where iqch.InspectionID = @InspectionId

	if @InspectionResult = 'Accepted' and @totalNGQty > 0
	begin
		declare @currentStock numeric(18,9) = 
		(
			SELECT SUM(sd.Qty) 
			FROM StockDetail sd
			INNER JOIN IQC_SamplingBarcodeDetail qc ON sd.BarcodeNo = qc.BarcodeNo
			WHERE qc.InspectionID = @InspectionId AND sd.Qty > 0
		)
		if isnull(@currentStock, 0) < @totalNGQty
		begin
			raiserror('Sisa stock kurang dari total NG!', 16,1)
			return
		end
	end

	/* 
		DISINI HARUS NYA ADA VALIDASI CEK, BARCODE2 YG ADA DI QC INI ADA YG LG DIPAKE GA DI TRANSAKSI KAYA MATERIAL REQUEST / CONSUME
		KHUSUS YG SOURCE NYA 'MATERIAL NG' AJA
	*/

	update IQC_Inspection_Header 
	set 
		InspectionResultSA = @InspectionResult, InspectionResultSAApproval = @UserId, InspectionResultSADate = getdate(), RemarksSA = @RemarksSA,
		LastUpdate = getdate(), StatusQC = 'CONFIRMED' 
	where InspectionID = @InspectionId

	declare @status varchar(50) = case when @InspectionResult = 'Accepted' then 'OK'  when @InspectionResult = 'Rejected' then 'NG' else 'HOLD' end

	declare @prefixFactory varchar(5) = (select PrefixGlobalBarcode from Company_Profile where Company_Code = @factoryCode)
	declare @prefixBarcode varchar(20) = @prefixFactory + 'NG' + format(@receiptDate, 'yyyyMMdd')

	declare @calcPartialNG table(Urutan int, RefNo varchar(50), WarehouseCode varchar(25), AreaCode varchar(25), AddressCode varchar(25), BarcodeNo varchar(50), LotNo varchar(100), ItemCode varchar(25), QtyAfter numeric(18,9), QtyNG numeric(18,9), InventoryQty numeric(18,9))
	declare @lotNoCalcPartialNG varchar(100), @warehouseCalcPartialNG varchar(25), @NewBarcodePartialNG varchar(50)

	declare @toWarehouse varchar(25) = (select top 1 WarehouseCode from PartReceiptDetailBarcode where ReceiptId = @ReceiptId and ItemCode = @ItemCode)

	declare @tblFullNG table 
	(
		Urutan int,
		RefNo varchar(50), 
		FromWarehouseCode varchar(50), 
		FromAreaCode varchar(25), 
		FromAddressCode varchar(25), 
		ItemCode varchar(25), 
		BarcodeNo varchar(50), 
		LotNo varchar(100), 
		SublotNo int, 
		Qty numeric(18,9),
		InventoryQty numeric(18,9)
	)
	declare @loopRef varchar(100), @loopFromWH varchar(25), @loopFromArea varchar(25), @loopFromAdddress varchar(25), 
			@loopItem varchar(25), @loopLot varchar(100), @loopQty numeric(18, 9), @loopQtyNG numeric(18, 9), @loopIVTQty numeric(18,9), @loopBarcode varchar(50), @loopSublot int

	declare @i int = 1, @transDate date = getdate()
	DECLARE	@NewRefNo varchar(50), @prefixPallet varchar(20) = 'PLT.' + @prefixFactory + '.' + 