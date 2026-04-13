SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE   PROCEDURE [sp_Wms_Mobile_SupplySubcon_GetDataBarcode]
	@BarcodeNo varchar(100),
	@RequestNo Varchar(100), 
	@ClassificationCode varchar(25)
as
begin
 
	Declare @ItemCode Varchar(100)='', @RequestDetailID bigint, @PONumber varchar(100), @SupplierCode varchar(25), @RequestStatusID int, @warehouse varchar(25)

	if not exists (select 1 from StockDetail where BarcodeNo = @BarcodeNo and Qty > 0)
	begin
		raiserror('Data Stock tidak ditemukan!', 16,1)
		return
	end

	if not exists (select 1 from StockDetail where BarcodeNo = @BarcodeNo and Qty > 0 and StatusReceipt = 'OK')
	begin
		raiserror('Status Stock harus OK!', 16,1)
		return
	end

	Select @ItemCode = ItemCode, @warehouse = WarehouseCode from StockDetail where BarcodeNo= @BarcodeNo and Qty > 0

	if @warehouse in (select Subcon_WH_Code from Trade_Master where Trade_Cls = '3') 
	begin
		raiserror('Lokasi stock berada di warehouse subcon!', 16,1)
		return
	end

	select 
		@RequestDetailID = a.RequestDetailID, @PONumber = b.PO_NO, @SupplierCode = po.Supplier_Code, @RequestStatusID = RequestStatusID
	from PartMaterialRequestDetail_PO a
	inner join PartMaterialRequestHeader_PO b on a.RequestID = b.RequestID 
   	inner join PurchaseOrder_Master po on b.PO_NO = po.PO_No
	where a.RefNumber = @RequestNo and a.AreaCode = @ClassificationCode

	if @RequestStatusID = 5
	BEGIN
		raiserror('Status Request No already complete!', 16,1)
		return
	end

	if exists 
	(
		select 1 from PartMaterialRequestItemDetailScan_PO a 
		inner join PartMaterialRequestItemDetail_PO b on a.IDSeq = b.IDSeq
		where b.RequestDetailID = @RequestDetailID and BarcodeNo = @BarcodeNo
	)
	BEGIN
		raiserror('Barcode already Scan!', 16,1)
		return
	end
	  
	declare @planQty numeric(18,9) = (select ChildRequirement_Qty From PartMaterialRequestItemDetail_PO where RequestDetailID = @RequestDetailID and ItemCode = @ItemCode)

	declare @scanQty numeric(18,9) = 
	(
		select sum(Qty) From PartMaterialRequestItemDetailScan_PO a
		inner join PartMaterialRequestDetail_PO b on a.IDSeq = a.IDSeq
		where b.RequestDetailID = @RequestDetailID and a.ItemCode = @ItemCode
	)

	SELECT  
		sd.WarehouseCode, 
		sd.BarcodeNo, 
		sd.LotNo, 
		sd.ItemCode, 
		sd.ProductionDate,
		mi.Item_Name ItemName,  
		isnull(@planQty, 0) PlanQty,  
		isnull(@scanQty, 0) QtyScan,  
		sd.Qty CurrentQty
	FROM StockDetail sd
	left join Item_Master mi on sd.ItemCode = mi.Item_Code	
	left join Unit_Cls uc on mi.Unit_Cls =  uc.Unit_Cls
	WHERE sd.BarcodeNo = @BarcodeNo and sd.Qty > 0
end
GO
