
create   procedure [dbo].[sp_Wms_ReceiptSupplyHistory_GetList]
--DECLARE
	@WarehouseCode varchar(25) = 'WH-001',
	@AreaCode varchar(25) = 'TMP',
	@ItemCode varchar(25) = '888997',
	@LotNo varchar(100) = 'ALL',
	@Period date = null
as
begin
	
	if @Period is null
	begin
		set @Period = cast(format(getdate(), 'yyyy-MM') + '-01' as date)
	end
	
	/* NOTE!!!
		1. TABLE ReceiptSupplyHistory untuk MENYIMPAN pergerkan BARCODE DISETIAP TRANSAKSI YG MERUBAH StockHeader.
			Status				= IN / OUT.
			ProcessMenu			= Proses Menu apa
			RefNo				= ini RefNo dari StockHeader & StockDetail
			WarehouseCode		= kalo status IN, ini diisi Warehouse TUJUAN. kalo ini diisi OUT diisi Warehouse ASAL.
			AreaCode			= kalo status IN, ini diisi Area TUJUAN. kalo ini diisi OUT diisi Area ASAL.
			AddressCode			= kalo status IN, ini diisi Address TUJUAN. kalo ini diisi OUT diisi Address ASAL.
			ItemCode			= kalo status IN, ini diisi Item TUJUAN. kalo ini diisi OUT diisi Item ASAL => ini buat handle kalo ada perubahan item kaya assy -> checker.
			BarcodeNo			= kalo status IN, ini diisi Barcode TUJUAN. kalo ini diisi OUT diisi Barcode ASAL => ini buat handle kalo ada split barcode.
			LotNo				= kalo status IN, ini diisi Lot TUJUAN. kalo ini diisi OUT diisi Lot ASAL => ini buat handle kalo ada split barcode / ubah lot.
			RefWarehouseCode	= kalo status IN, ini diisi Warehouse ASAL. kalo ini diisi OUT diisi Warehouse TUJUAN.
			RefAreaCode			= kalo status IN, ini diisi Area ASAL. kalo ini diisi OUT diisi Area TUJUAN.
			RefAddressCode		= kalo status IN, ini diisi Address ASAL. kalo ini diisi OUT diisi Address TUJUAN.
			RefItemCode			= kalo status IN, ini diisi Item ASAL. kalo ini diisi OUT diisi Item TUJUAN => ini buat handle kalo ada perubahan item kaya assy -> checker.
			RefBarcodeNo		= kalo status IN, ini diisi Barcode ASAL. kalo ini diisi OUT diisi Barcode TUJUAN => ini buat handle kalo ada split barcode.
			RefLotNo			= kalo status IN, ini diisi Lot ASAL. kalo ini diisi OUT diisi Lot TUJUAN => ini buat handle kalo ada split barcode / ubah lot.
			QtyTrans			= Qty Transaksi
			Remarks				= ini terserah isi apa
			ReferenceNo			= ini juga terserah diisi apa
			LogDate				= tgl log nya, di transaksi pake getdate()
			UserID				= pelaku nya
		2. Harus nya filter nya cuma sampe Warehouse, artinya kalo ada berubah2 area ga perlu di tampilin.
	*/
	select 
		res.*,
		isnull(sm.TMPreMonth,0) PreMonth,
		case when res.TransactionType = 'IN' then isnull(res.QtyTrans,0) else 0 end Receipt,
		case when res.TransactionType = 'OUT' then isnull(res.QtyTrans,0) else 0 end Supply,
		isnull(sm.TMLossReject,0) Reject,
		isnull(sm.TMCurrent,0) [Current],	
		mi.Item_Name ItemName, 
		case when res.TransactionType = 'OUT' THEN isnull(ma.AreaCode, '') else isnull(maf.AreaCode, '') end FromAreaName, 
		case when res.TransactionType = 'IN' THEN isnull(ma.AreaCode, '') else isnull(maf.AreaCode, '') end ToAreaName, 
		us.FullName LastUser 
	From 
	(
		select Status TransactionType, ProcessMenu, WarehouseCode, AreaCode, ItemCode, RefWarehouseCode, RefAreaCode, LotNo, Remarks, ReferenceNo [DocReference], format(LogDate, 'yyyy-MM-dd HH:mm') TransactionDate, UserID, SUM(QtyTrans) QtyTrans from ReceiptSupplyHistory
		where 1=1
		AND MONTH(LogDate) = MONTH(@Period) AND YEAR(LogDate) = YEAR(@Period)
		AND 
		(
			/*Query ini karna filter nya cuma sampe Warehouse, artinya kalo ada berubah2 area ga perlu di tampilin, karna cukup tampilin yg kalo berubah2 di warehouse aja*/
			WarehouseCode <> isnull(RefWarehouseCode, '')
			/*Tambah query ini kalo filter nya sampe area, artinya perlu cek baik itu ada berubah warehouse maupun area 
			*/
			OR AreaCode <> isnull(RefAreaCode, '')
			/*
			*/
		)
		AND (@LotNo = 'ALL' or LotNo = @LotNo)
		group by Status, ProcessMenu, WarehouseCode, AreaCode, ItemCode, RefWarehouseCode, RefAreaCode, LotNo, Remarks, ReferenceNo, format(LogDate, 'yyyy-MM-dd HH:mm'), UserID
	) res
	LEFT JOIN 
	(
		SELECT 
			WarehouseCode, AreaCode, ItemCode, LotNo, 
			isnull(sum(TMPreMonth),0) TMPreMonth,
			isnull(sum(TMLossReject),0) TMLossReject,
			isnull(sum(TMCurrent),0) TMCurrent
		fROM StockHeader 
		WHERE WarehouseCode = @WarehouseCode 
		AND AreaCode = @AreaCode
		and ItemCode = @ItemCode
		AND (@LotNo = 'ALL' or LotNo = @LotNo)
		group by WarehouseCode, AreaCode, ItemCode, LotNo
	) sm on res.AreaCode = sm.AreaCode and res.LotNo = sm.LotNo
	LEFT JOIN Item_Master mi on res.ItemCode = mi.Item_Code
	LEFT JOIN vw_WarehouseLine mw on res.WarehouseCode = mw.WarehouseCode
	LEFT JOIN vw_WarehouseLine mwf on res.RefWarehouseCode = mwf.WarehouseCode
	LEFT JOIN vw_Area ma on res.AreaCode = ma.AreaCode
	LEFT JOIN vw_Area maf on res.RefAreaCode = maf.AreaCode
	LEFT join SS_UserSetup us on res.UserID = us.UserID
	WHERE res.WarehouseCode = @WarehouseCode 
	AND res.AreaCode = @AreaCode
	and res.ItemCode = @ItemCode
	ORDER BY TransactionDate
end
