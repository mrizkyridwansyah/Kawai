CREATE PROCEDURE [dbo].[sp_Wms_Receipt_Import]
      @SupplierCode VARCHAR(15)
    , @DNNumber VARCHAR(50)			
    , @ReceiptDate DATE				
    , @BCType VARCHAR(15)			
    , @BCNumber VARCHAR(50)			
    , @BCDate DATE					
	, @DataImport tvp_ReceiptImport READONLY 
	, @UserId varchar(25)
	, @FactoryCode Varchar(10) 
as
begin	
	DECLARE @ResultHeader TABLE(
	    SupplierCode VARCHAR(15),
        DNNumber VARCHAR(50),
        ReceiptDate DATE,
        BCType VARCHAR(15),
        BCNumber VARCHAR(50),
        BCDate DATE,
		Errors varchar(max)
	)

	DECLARE @ResultDetail TABLE(
	    [PONumber] [varchar](25) NOT NULL,
	    [ItemCode] [varchar](25) NOT NULL,
	    [ReceiptQty] [int] NULL,
		[RowNumber] [int] NULL,
		[Errors] varchar(max)
	)

	insert into @ResultHeader
	select  @SupplierCode , @DNNumber , @ReceiptDate , @BCType , @BCNumber , @BCDate , ''

	insert into @ResultDetail
	select [PONumber] ,[ItemCode] ,[ReceiptQty] ,ROW_NUMBER() OVER (ORDER BY [PONumber], [ItemCode]) , [Errors] from  @DataImport

	declare @warehouseSubcon varchar(25), @tradeCls varchar(3) 
	select @warehouseSubcon = Subcon_WH_Code, @TradeCls = Trade_Cls from Trade_Master where Trade_Code = @SupplierCode

 
    
	DECLARE	@ReceiptNo varchar(50), @prefixReceiptNo varchar(10) = 'R.' + FORMAT(@ReceiptDate, 'yyyyMMdd') + '.'
	EXEC dbo.GenerateNumerator @Prefix = @prefixReceiptNo, @LengthSequence = 4, @Result = @ReceiptNo OUTPUT;			

 

 
 if @tradeCls = '3'
	begin
		declare @tblRequirementMaterial table (PONumber varchar(100), ParentItem varchar(25), ItemCode varchar(25), BOMQty numeric(18,9), ReceiptQty numeric(18,9))

		insert into @tblRequirementMaterial
		select dtl.PONumber, dtl.ItemCode, bom.Item_Code, bom.Qty, dtl.ReceiptQty
		From BOM_Master bom 
		inner join @ResultDetail dtl on bom.Parent_ItemCode = dtl.ItemCode

		declare @pickingNos table (PONumber varchar(100), ParentItem varchar(25), PickingNo varchar(100))
		insert into @pickingNos
		select distinct hd.PO_NO, hd.ParentItem_Code, dtl.RefNumber From PartMaterialRequestDetail_PO dtl
		inner join PartMaterialRequestHeader_PO hd on dtl.RequestID = hd.RequestID
		inner join PartMaterialRequestItemDetail_PO dtli on dtl.RequestDetailID = dtli.RequestDetailID
		inner join @ResultDetail req on hd.PO_NO = req.PONumber

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
	end

	begin transaction receiptTransaction
	begin try
		DECLARE @registerNox TABLE (RegisterNo VARCHAR(100))
		DECLAre @registerNo varchar(100)

		IF @BCType NOT IN ('BC 2.3', 'BC 2.6.2', 'BC 4.0')
		BEGIN
			INSERT INTO @registerNox
			EXEC sp_GetNoRegister @ReceiptDate, 'R', @UserId

			SELECT top 1 @registerNo = RegisterNo FROM @registerNox
		END

		insert into PartReceiptHeader 
		(ReceiptNo, ReceiptDate, SupplierCode, DNNumber, DNDate, BCNumber, BCType, BCDate, VehicleNo, RegisterDate, RegisterUser, IsManual, Transport, Remarks, SourceMenu, CompanyCode, RegisterNo)
		values 
		(@ReceiptNo, @ReceiptDate, @SupplierCode, @DNNumber, @BCDate, @BCNumber, @BCType, @BCDate, '', getdate(), @UserId, 1, NULL, '', 'RECEIPT PO', @FactoryCode, @registerNo)

		declare @newid bigint = (select SCOPE_IDENTITY())

		insert into PartReceiptDetail (ReceiptId, ReceiptDate, PONumber, ItemCode, UnitCls, ExpectedQty, TotalPacking, ReceiptQty, Remarks, QtyPacking, NoSeri)
		select 
			@newid, @ReceiptDate, a.PONumber, a.ItemCode, '01', a.ReceiptQty, CEILING(CAST(a.ReceiptQty AS FLOAT) / isnull(isp.QtyPacking, mi.Number_Box)), a.ReceiptQty, '', isnull(isp.QtyPacking, mi.Number_Box), a.RowNumber
		from @ResultDetail a 
		left join ItemSupplierPacking isp on a.ItemCode = isp.ItemCode and isp.SupplierCode = @SupplierCode
		left join Item_Master mi on a.ItemCode = mi.Item_Code

		declare @seqNo int = (isnull((select max(Seq_No) From Part_Receipt with (updlock, holdlock)), 0))
		DECLARE @BCTypeVal varchar(100) = (SELECT Description fROM BCType_Cls	WHERE BCType_Cls = @BCType)

		insert into Part_Receipt 
		(
			Seq_No, Supplier_Code, PO_No, Warehouse_Code, Address, Receipt_Cls, Receipt_Date, Item_Code, Qty, SerialNoFrom, SerialNoTo, 
			Unit_Cls, Currency_Code, Price, Amount, SuratJalan_No, ProductionResult_Cls, DailySeq_No, Remarks, Transport_Cls,
			Last_Update, Last_User, Register_Date, BC_Type, BC40_No, BC40_Date, Receipt_Status, No_Register, RefWMSReceiptId, No_Seri
		)
		select 
			@seqNo + ROW_NUMBER() OVER (ORDER BY dtl.Id), hd.SupplierCode, dtl.PONumber, ISNULL(POH.WHTo, it.WH_Code), '' [Address], 'R', @ReceiptDate, dtl.ItemCode, dtl.ReceiptQty, null SerialNoFrom, null SerialNoTo,  
			dtl.UnitCls, pod.Currency_Code, pod.Price, pod.Price * dtl.ReceiptQty, hd.DNNumber, 0, null DailySeq_No, '', NULL,
			getdate(), @UserId, getdate(), isnull(@BCTypeVal, hd.BCType), hd.BCNumber, hd.BCDate, null Receipt_Status, @registerNo, hd.Id, dtl.NoSeri
		From PartReceiptHeader hd
		inner join PartReceiptDetail dtl on hd.Id = dtl.ReceiptId
		left join Item_Master it on dtl.ItemCode = it.Item_Code
		left join PurchaseOrder_Detail pod on dtl.ItemCode = pod.Item_Code and dtl.PONumber = pod.PO_No
		left join PurchaseOrder_Master poh on poh.PO_No = pod.PO_No
		where hd.Id = @newid

		select @newid

		commit transaction receiptTransaction
	end try
	begin catch
		rollback transaction receiptTransaction
		declare @msgErr varchar(max) = (select ERROR_MESSAGE())
		raiserror(@msgErr, 16, 1)
		return
	end catch
end
