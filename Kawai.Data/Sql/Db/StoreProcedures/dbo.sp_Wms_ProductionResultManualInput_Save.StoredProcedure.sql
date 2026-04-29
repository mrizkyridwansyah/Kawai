 
CREATE Procedure [dbo].[sp_Wms_ProductionResultManualInput_Submit]
    @LineCode VARCHAR(25) = 'DP-01',
	@NewRequest tvp_ProductionResultManualInput READONLY,
	@UserId varchar(25)

  as


/* ==========================================================
   VARIABLE
========================================================== */
DECLARE
    @ProdResultID BIGINT,
    @ProductionID BIGINT,
    @ProductionDate DATE,
    @ItemCode VARCHAR(100),
    @PlanQty INT,
    @ResultQty INT,
    @QtyBox INT = 10,
    @TotalDone INT = 0,
    @LoopQty INT,
    @DetailLoop INT,
    @MaxDetail INT


/*==========================================================
   GET DATA
========================================================== */
SELECT
    @ProductionID   = A.ProductionId,
    @ProductionDate = A.ScheduleDate,
    @ResultQty      = A.ResultQty,
    @ItemCode       = B.Item_Code,
    @PlanQty        = C.Qty
FROM @NewRequest A
LEFT JOIN Item_Master B ON A.ItemCode = B.Item_Code
LEFT JOIN Daily_Production C ON C.Seq_No = A.ProductionId


DECLARE @RefNo VARCHAR(50),@Workstation varchar(50),@Stoppoint VARCHAR(50)
		SELECT @RefNo=c.RefNumber, @Workstation=c.WorkStationCode,@Stoppoint=e.StopPointCode
		FROM dbo.PartMaterialRequestItemDetailScan a 
		LEFT JOIN PartMaterialRequestItemDetail b ON b.IDSeq=a.IDSeq
		LEFT JOIN PartMaterialRequestDetail c ON c.RequestDetailID=b.RequestDetailID
		LEFT JOIN PartMaterialRequestHeader d ON d.RequestID=c.RequestID
		LEFT JOIN WorkStationLineSetting e ON e.LineCode=d.LineCode and e.WorkStationCode=c.WorkStationCode
		WHERE D.ParentItem_Code=@ItemCode AND D.ProductionID=@ProductionID


/* ==========================================================
   HEADER
========================================================== */
IF NOT EXISTS
(
    SELECT 1
    FROM ProductionResultHeader
    WHERE ProductionID = @ProductionID
)
BEGIN

    INSERT INTO ProductionResultHeader
    (
        ProductionID,
        ProductionDate,
        ItemCode,
        Shift,
        TotalGoodQty,
        TotalNGQty,
        RegisterDate,
        RegisterUser
    )
    VALUES
    (
        @ProductionID,
        @ProductionDate,
        @ItemCode,
        NULL,
        0,
        0,
        GETDATE(),
        @UserId
    )

    SET @ProdResultID = SCOPE_IDENTITY()

END
ELSE
BEGIN

    SELECT
        @ProdResultID = ProdResultID,
        @TotalDone    = ISNULL(TotalGoodQty,0)
    FROM ProductionResultHeader
    WHERE ProductionID = @ProductionID

END



/* ==========================================================
   JIKA SUDAH FULL
========================================================== */
IF @TotalDone >= @PlanQty
BEGIN

    UPDATE Daily_Production
    SET Complete_Cls = 1
    WHERE Seq_No = @ProductionID

    RAISERROR( 'Production already completed', 16,1)
    RETURN

END



/* ==========================================================
   AMBIL QTY YANG BOLEH DIINPUT
========================================================== */
SET @LoopQty =
CASE
    WHEN (@PlanQty - @TotalDone) >= @ResultQty
        THEN @ResultQty
    ELSE (@PlanQty - @TotalDone)
END



/* ==========================================================
   HITUNG DETAIL BOX
========================================================== */
SET @MaxDetail = CEILING(@LoopQty * 1.0 / @QtyBox)
SET @DetailLoop = 1



/* ==========================================================
   INSERT DETAIL
========================================================== */
WHILE @DetailLoop <= @MaxDetail
BEGIN

    DECLARE @factoryCode VARCHAR(25)
    DECLARE @prefixFactory VARCHAR(5)
    DECLARE @NewBarcode VARCHAR(50)
    DECLARE @dt VARCHAR(8)
    DECLARE @prefixBarcode VARCHAR(30)


	SELECT TOP 1 @factoryCode = Company_Code from Manufacture_Line where Line_Code = @LineCode
    SELECT @prefixFactory = PrefixGlobalBarcode
    FROM Company_Profile
    WHERE Company_Code = @factoryCode

    SET @dt = FORMAT(GETDATE(),'yyyyMMdd')
    SET @prefixBarcode = @prefixFactory + 'MP' + @dt

    EXEC dbo.GenerateNumerator
        @Prefix = @prefixBarcode,
        @LengthSequence = 4,
        @Result = @NewBarcode OUTPUT
   DECLARE @Date date = GetDate()	
   Declare @LotNo Varchar(100) = 'L-' + CAST(@ProductionID AS VARCHAR)+'.' + FORMAT(@ProductionDate,'yyyyMMdd')
   Declare @QtyDetail int
   Select  @QtyDetail = CASE WHEN @DetailLoop = @MaxDetail AND (@LoopQty % @QtyBox) <> 0 THEN (@LoopQty % @QtyBox)
				             ELSE @QtyBox END 
    INSERT INTO ProductionResultDetail
    (
        ProdResultID,
        BarcodeNo,
        LotNo,
        SerialNo,
        Qty,
        ResultType,
        RegisterDate,
        RegisterUser
    )
    VALUES
    (
        @ProdResultID,
        @NewBarcode,
        @LotNo,
        NULL,
        @QtyDetail,
        'HOLD',
        GETDATE(),
        @UserId
    )

	   /* ==========================================================
         INSERT STOCK DETAIL
         ========================================================== */

	    INSERT INTO StockDetail
		( RefNo, WarehouseCode, AreaCode, AddressCode, BarcodeNo, ItemCode, LotNo, SublotNo, Qty, InventoryQty, ExpiredDate, ProductionDate, ReceiptDate, Supplier, PrintCls, 
		  DisposalCls, StatusReceipt, Picking_No, RegisterDate, RegisterUser)
		SELECT  TOP 1 RefNo=@RefNo, @LineCode, 
		             AreaCode=@Workstation, AddressCode=@Stoppoint, 
					 @NewBarcode, @ItemCode, @LotNo,  SublotNo=NULL,
					 @QtyDetail,   
					 InventoryQty= NULL , ExpiredDate=NULL, ProductionDate=@ProductionDate, 
					 ReceiptDate=NULL, Supplier=NULL, PrintCls=NULL, 
					 DisposalCls=NULL, StatusReceipt=NULL, Picking_No=NULL, GETDATE(), @UserId 
	 
	    /* ==========================================================
         UPSERT STOCK HEADER
         ========================================================== */

	     EXEC sp_Wms_Stock_UpSertStockHeader @Date, @RefNo,  @LineCode, @Workstation, @ItemCode, @LotNo, @QtyDetail, NULL, 'R', @UserId
        
		 /* ==========================================================
         INSERT CONSUMPTION & UPDATE STOCK
         ========================================================== */

	     EXEC sp_Wms_MaterialConsumption_Insert @LineCode=@LineCode,@ParentItem=@ItemCode,@ProductionID=@ProductionID,@QtyResult=@QtyDetail,@ResultDetailID=@ProdResultID,@UserID=@UserID
	 

    SET @DetailLoop = @DetailLoop + 1

END



/* ==========================================================
   UPDATE HEADER
========================================================== */
UPDATE ProductionResultHeader
SET TotalGoodQty = ISNULL(TotalGoodQty,0) + @LoopQty,
    LastUpdate = GETDATE(),
    LastUser = @UserId
WHERE ProdResultID = @ProdResultID



/* ==========================================================
   COMPLETE CHECK
========================================================== */
SELECT @TotalDone = ISNULL(TotalGoodQty,0)
FROM ProductionResultHeader
WHERE ProdResultID = @ProdResultID


IF @TotalDone >= @PlanQty
BEGIN
    UPDATE Daily_Production
    SET Complete_Cls = 1
    WHERE Seq_No = @ProductionID
END
ELSE
BEGIN
    UPDATE Daily_Production
    SET Complete_Cls = 0
    WHERE Seq_No = @ProductionID
END


