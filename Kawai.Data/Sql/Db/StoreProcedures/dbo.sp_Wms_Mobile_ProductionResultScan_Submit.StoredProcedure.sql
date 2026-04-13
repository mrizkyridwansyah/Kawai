SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--exec sp_Wms_Mobile_ProductionResultScan_Submit @BarcodeNo=N'101J0170J10000-G099657',@LotNo=N'23',@ItemCode=N'101J0170',@LineCode=N'001',@Qty=1.0,@UserId=N'admin'
CREATE   PROCEDURE [sp_Wms_Mobile_ProductionResultScan_Submit]
	@BarcodeNo	NVARCHAR(50),
	@LotNo		NVARCHAR(50),
	@ItemCode	NVARCHAR(50),
	@LineCode	NVARCHAR(50),
	@Qty		NUMERIC(18,9)=1,
	@UserID		NVARCHAR(35)
AS
BEGIN

	--DECLARE @BarcodeNo	NVARCHAR(50)='101J0170J10000-G099657',
	--		@LotNo			NVARCHAR(50)='23',
	--		@ItemCode		NVARCHAR(50)='101J0170',
	--		@LineCode		NVARCHAR(50)='001',
	--		@Qty			NUMERIC(18,9)=1,
	--		@UserID			NVARCHAR(35)='admin'
	
	IF EXISTS(SELECT TOP 1 1 FROM ProductionResultDetail WHERE BarcodeNo=@BarcodeNo)
	BEGIN
		RAISERROR('Barcode No has been scanned',16,1)
		RETURN
	END


	DECLARE @SerialNo NVARCHAR(25)
	
	SELECT TOP 1 @SerialNo=PartValue FROM dbo.fn_ProductionResult_GetSerialNo(@BarcodeNo)
	WHERE PartNo=2

	DECLARE @Daily_Production AS TABLE(Seq_No NUMERIC(18,9), Line_Code CHAR(15), Item_Code CHAR(25),Schedule_Date DATE, Lot_No CHAR(7) )

	INSERT INTO @Daily_Production(Seq_No,Line_Code,Item_Code,Schedule_Date,Lot_No)
	SELECT A.Seq_No,A.Line_Code,A.Item_code,A.Schedule_Date,A.Lot_No
	FROM Daily_Production a
	LEFT JOIN Item_Master B ON B.Item_Code=A.Item_code
	WHERE Complete_Cls is null 
	AND @SerialNo>=SerialNoFrom and @SerialNo<=SerialNoTo

	DECLARE @SeqNo BIGINT
	SELECT TOP 1 @SeqNo=A.Seq_No
	FROM @Daily_Production A

	IF NOT EXISTS(SELECT TOP 1 1 FROM @Daily_Production)
	BEGIN 
		RAISERROR('Serial No not found in daily production!',16,1)
		RETURN
	END
	
	--SET NOCOUNT ON;
	--SET XACT_ABORT ON;  -- ensure transaction aborts on error
	DECLARE @ErrorMessage NVARCHAR(MAX)
	BEGIN TRY
		SET TRANSACTION ISOLATION LEVEL READ COMMITTED

		BEGIN TRANSACTION myTran

		INSERT INTO ProductionResultHeader
		( ProductionID, ProductionDate, ItemCode, Shift, TotalGoodQty, TotalNGQty, RegisterDate, RegisterUser)
		SELECT TOP 1 A.Seq_No,A.Schedule_Date,@ItemCode,NULL,1,NULL,GETDATE(),@UserID
		FROM @Daily_Production A
	
		DECLARE @NewID bigint = (select SCOPE_IDENTITY())
	
		INSERT INTO ProductionResultDetail
		(ProdResultID, BarcodeNo, LotNo, SerialNo, Qty, ResultType, Registerdate, RegisterUser)
		VALUES (@NewID,@BarcodeNo,@LotNo,@SerialNo,@Qty,'HOLD',GETDATE(),@UserID)

		DECLARE @ProdResultID bigint = (select SCOPE_IDENTITY())
		--PRINT(' prod result : '+cast(@ProdResultID as varchar))
		
		DECLARE @RefNo VARCHAR(50),@Workstation varchar(50),@Stoppoint VARCHAR(50)
		SELECT @RefNo=c.RefNumber, @Workstation=c.WorkStationCode,@Stoppoint=e.StopPointCode
		FROM dbo.PartMaterialRequestItemDetailScan a 
		LEFT JOIN PartMaterialRequestItemDetail b ON b.IDSeq=a.IDSeq
		LEFT JOIN PartMaterialRequestDetail c ON c.RequestDetailID=b.RequestDetailID
		LEFT JOIN PartMaterialRequestHeader d ON d.RequestID=c.RequestID
		LEFT JOIN WorkStationLineSetting e ON e.LineCode=d.LineCode and e.WorkStationCode=c.WorkStationCode
		WHERE D.ParentItem_Code=@ItemCode AND D.ProductionID=@SeqNo
				
		INSERT INTO StockDetail
		( RefNo, WarehouseCode, AreaCode, AddressCode, BarcodeNo, ItemCode, LotNo, SublotNo, Qty, InventoryQty, ExpiredDate, ProductionDate, ReceiptDate, Supplier, PrintCls, 
		  DisposalCls, StatusReceipt, Picking_No, RegisterDate, RegisterUser)
		SELECT  TOP 1 RefNo=@RefNo, @LineCode, AreaCode=@Workstation, AddressCode=@Stoppoint, BarcodeNo, @ItemCode, LotNo,  SublotNo=NULL, Qty,InventoryQty= NULL , ExpiredDate=NULL, ProductionDate=Schedule_Date, 
				ReceiptDate=NULL, Supplier=NULL, PrintCls=NULL, 
				DisposalCls=NULL, StatusReceipt=NULL, Picking_No=NULL, GETDATE(), @UserID 
		FROM ProductionResultDetail A
		JOIN @Daily_Production B ON B.Lot_No=a.LotNo
		WHERE BarcodeNo=@BarcodeNo

		DECLARE @Date date = GetDate()	
	
		exec sp_Wms_Stock_UpSertStockHeader @Date, @RefNo,  @LineCode, @Workstation, @ItemCode, @LotNo, 1, NULL, 'R', @UserId

		BEGIN TRY
			----INSERT CONSUMPTION & UPDATE STOCK-----
			EXEC sp_Wms_MaterialConsumption_Insert @LineCode=@LineCode,@ParentItem=@ItemCode,@ProductionID=@SeqNo,@QtyResult=@Qty,@ResultDetailID=@ProdResultID,@UserID=@UserID
		END TRY
		BEGIN CATCH
			SET @ErrorMessage=ERROR_MESSAGE()
			RAISERROR(@ErrorMessage,16,1)
		END CATCH

		COMMIT TRANSACTION myTran
	END TRY
	BEGIN CATCH
		
		--IF XACT_STATE() <> 0
		ROLLBACK TRANSACTION myTran
		RAISERROR(@ErrorMessage,16,1)
		PRINT(@ErrorMessage)
	END CATCH

end
GO
