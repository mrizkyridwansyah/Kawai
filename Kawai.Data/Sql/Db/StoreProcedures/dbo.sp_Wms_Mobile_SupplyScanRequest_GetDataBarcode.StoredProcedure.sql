

CREATE   PROCEDURE [dbo].[sp_Wms_Mobile_SupplyScanRequest_GetDataBarcode]
	@BarcodeNo varchar(100),
	@RequestNo Varchar(100), 
	@ItemClass Varchar(25) 
as
begin
 
	Declare @ItemCode Varchar(100)='', @RequestDetailID bigint
	DECLARE @Warehouse varchar(25), @Area varchar(25), @Address varchar(25), @PickingNo varchar(100), @StatusReceipt varchar(100), @QtyStock numeric(18, 9)

    SELECT 
        @Warehouse = WarehouseCode, 
        @Area = AreaCode, 
        @Address = AddressCode, 
		@ItemCode = ItemCode,
		@PickingNo = Picking_No,
		@StatusReceipt = StatusReceipt,
        @QtyStock = ISNULL(Qty,0)
    FROM dbo.StockDetail 
    WHERE BarcodeNo = @BarcodeNo AND ISNULL(Qty,0) > 0

	if isnull(@QtyStock, 0) <= 0
	begin
		raiserror('Data Stock tidak ditemukan!', 16,1)
		return
	end

	if @Warehouse in (select Subcon_WH_Code from Trade_Master where Trade_Cls = '3') 
	begin
		raiserror('Lokasi stock berada di warehouse subcon!', 16,1)
		return
	end

	if @StatusReceipt <> 'OK'
	begin
        RAISERROR('Status Barcode belum OK',16,1)
        RETURN
	end

	if isnull(@PickingNo, '') <> ''
	begin
		raiserror('Barcode Already used to picking process', 16,1)
        RETURN
	end

	select @RequestDetailID = RequestDetailID from PartMaterialRequestDetail where RefNumber = @RequestNo and AreaCode = @ItemClass
   
   -- ini dimatiin, kalo kaya gini nanti barcode yg pernah discan ga bisa di pake terus
	--if exists (select 1 from PartMaterialRequestItemDetailScan A  where BarcodeNo = @BarcodeNo)
	--BEGIN
	--	raiserror('Barcode already Scan!', 16,1)
	--	return
	--end
	  
	--if not exists (select * from PartMaterialRequestItemDetail where RequestDetailID = @RequestDetailID and ItemCode = @ItemCode)
	--begin
	--	raiserror('Item tidak terdaftar dalam list material request!', 16,1)
	--	return
	--end

	if exists 
	(
		select 1 from StockDetail A 
		Left JOIN MS_Address B ON A.AddressCode = B.AddressCode 
		where BarcodeNo = @BarcodeNo 
		and  Qty > 0 
		and StatusReceipt = 'OK' 
		and StopPointCode is NULL
	)
	begin
		raiserror('Address barcode ini belum setting stop point', 16,1)
		return
	end
	
	declare @planQty numeric(18,9) = (select ChildRequirement_Qty From PartMaterialRequestItemDetail where RequestDetailID = @RequestDetailID and ItemCode = @ItemCode)

	declare @scanQty numeric(18,9) = 
	(
		select sum(Qty) From PartMaterialRequestItemDetailScan a
		inner join PartMaterialRequestDetail b on a.IDSeq = a.IDSeq
		where b.RequestDetailID = @RequestDetailID and a.ItemCode = @ItemCode
	)

	SELECT  
		sd.WarehouseCode, 
		sd.BarcodeNo, 
		Line_Code LineCode, 
		'' RequestNo, 
		'' ProductionDate, 
		LotNo, 
		CAST(isnull(CAST(@planQty as Numeric(18,0)), 0) as varchar)+' ' + uc.[Description] UnitDesc,
		sd.ItemCode, 
		mi.Item_Name ItemName,  
		isnull(@planQty, 0) PlanQty,  
		sd.Qty QtyScan
	FROM StockDetail sd
	left join Item_Master mi on sd.ItemCode = mi.Item_Code	
	left join Unit_Cls uc on mi.Unit_Cls =  uc.Unit_Cls
	WHERE sd.BarcodeNo = @BarcodeNo and sd.Qty > 0
end
