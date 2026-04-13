SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Mobile_MaterialNG_GetDataNG]
	@BarcodeNo varchar(100) 
as
begin
	if not exists (select 1 from PartReceiptDetailBarcode where BarcodeNo = @BarcodeNo)
	begin
		raiserror('Data barcode tidak ditemukan', 16, 1)
		return
	end

	if not exists (select 1 from StockDetail where BarcodeNo = @BarcodeNo and StatusReceipt = 'OK')
	begin
		raiserror('Status Barcode belum OK', 16, 1)
		return
	end

	if exists 
	(
		select 1 
		from StockDetail 
		where BarcodeNo = @BarcodeNo
		and Qty > 0 
		and WarehouseCode in 
		(
			select Subcon_WH_Code from Trade_Master where Trade_Cls = '3'
		) 
	)
	begin
		raiserror('Lokasi stock berada di warehouse subcon!', 16,1)
		return
	end

	declare @PONumber varchar(100), @ItemCode varchar(25)
	select @PONumber = PONumber, @ItemCode = ItemCode
	From PartReceiptDetailBarcode 
	where BarcodeNo = @BarcodeNo

	if exists 
	(
		select 1 from IQC_SamplingBarcodeDetail dtl
		inner join IQC_Inspection_Header hd on dtl.InspectionID = hd.InspectionID 
		where hd.PO_Number = @PONumber 
		and hd.ItemCode = @ItemCode 
		and hd.Soruce = 'Material NG' 
		and hd.StatusQC = 'CONFIRMED'
	)
	begin
		raiserror('Material NG dari PO barcode ini sudah diconfirm!', 16,1)
		return
	end

	SELECT
		smp.SamplingID SampleId, smp.InspectionID InspectionId,
		c.SupplierCode,
		sp.Trade_Name SupplierName,
		a.ItemCode,
		b.Item_Name [ItemName],
		a.BarcodeNo, a.LotNo, a.SublotNo, a.Qty,
		QtyNG = isnull(smp.SampleQTY, 0)
	FROM PartReceiptDetailBarcode a
	INNER JOIN PartReceiptHeader c on a.ReceiptId = c.Id
	LEFT JOIN Trade_Master sp on c.SupplierCode = sp.Trade_Code
	LEFT JOIN 
	(
		select xx.* from IQC_SamplingBarcodeDetail xx
		INNER JOIN IQC_Inspection_Header yy on xx.InspectionID = yy.InspectionID
		WHERE yy.Soruce = 'Material NG'
	) smp on a.BarcodeNo = smp.BarcodeNo
	LEFT JOIN Item_Master b ON a.ItemCode = b.Item_Code
	WHERE a.BarcodeNo = @BarcodeNo
end
GO
