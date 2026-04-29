
CREATE   PROCEDURE [dbo].[sp_Wms_MaterialConsumption_Insert]
	@LineCode		NVARCHAR(50),
	@ParentItem		VARCHAR(50),
	@ProductionID	BIGINT,
	@QtyResult		NUMERIC(18,5),
	@ResultDetailID	INT,
	@UserID			VARCHAR(50)
AS
BEGIN

	--DECLARE @LineCode		NVARCHAR(50)='001',
	--		@ParentItem		VARCHAR(50)='101J0170',
	--		@ProductionID	BIGINT=1020394,
	--		@QtyResult		NUMERIC(18,5)=1,
	--		@ResultDetailID	INT=15,
	--		@UserID			VARCHAR(50)='admin'

	DECLARE @LineNoConsumpt				VARCHAR(25),
			@WorkStationCode			VARCHAR(25),
			@StopPointCode				VARCHAR(25),
			@ParentItemCodeConsumpt		VARCHAR(25),					
			@ItemCodeConsumpt			VARCHAR(50),	
			@ItemNameConsumpt			VARCHAR(250),	
			@BarcodeNoConsumpt			VARCHAR(50),				
			@LotNoConsumpt				VARCHAR(50), 			
			@Qty						NUMERIC(18,9),
			@Used						NUMERIC(18,9),
			@FromRefNo					VARCHAR(50) = ''
						
	DECLARE @QtySisa					NUMERIC(18,9) = 0		
	DECLARE @Counter					INT = 1		
								
	DECLARE @PackingQty					NUMERIC(18,9)		
	DECLARE @QtyBOM						NUMERIC(18,9)
	DECLARE @ItemCodeBefore				VARCHAR(50) = ''
	DECLARE @Date date = GetDate()	
			

	DECLARE ConsumptCursor CURSOR FOR	
		
		SELECT LineCode,scan.WorkStationCode,StopPointCode,ParentItem_Code,scan.ItemCode,RTRIM(itm.Item_Name) ItemName,bom.QtyBom,scan.BarcodeNo,stock.LotNo,stock.QtyStock,COALESCE(QtyUsed,0) QtyUsed,RefNo as RefNoStock
		FROM (	SELECT c.RefNumber, d.LineCode,c.WorkStationCode,e.StopPointCode,D.ParentItem_Code,A.ItemCode,a.BarcodeNo,a.Qty,a.RegisterDate
				FROM dbo.PartMaterialRequestItemDetailScan a 
				LEFT JOIN PartMaterialRequestItemDetail b ON b.IDSeq=a.IDSeq
				LEFT JOIN PartMaterialRequestDetail c ON c.RequestDetailID=b.RequestDetailID
				LEFT JOIN PartMaterialRequestHeader d ON d.RequestID=c.RequestID
				LEFT JOIN WorkStationLineSetting e ON e.LineCode=d.LineCode and e.WorkStationCode=c.WorkStationCode
				WHERE D.ParentItem_Code=@ParentItem AND D.ProductionID=@ProductionID
			) scan 
		JOIN (
				SELECT RefNo,Picking_No,WarehouseCode,AreaCode,AddressCode,ItemCode,BarcodeNo,LotNo,Qty AS QtyStock FROM StockDetail
				WHERE COALESCE(Qty,0)>0
			 ) stock ON stock.WarehouseCode=scan.LineCode AND stock.AreaCode=scan.WorkStationCode AND stock.AddressCode=scan.StopPointCode 
				AND stock.ItemCode=scan.ItemCode AND stock.BarcodeNo=scan.BarcodeNo
				AND stock.Picking_No=RefNumber
		JOIN (
				SELECT a.Bomws_ID,a.Line_Code,a.ParentItemCode,a.WorkStationCode,c.ChildItem_Code,c.QtyBom FROM MS_BOMPerworkstation_Header A 
				JOIN (
						SELECT DetailID,Bomws_ID,ChildItem_Code,Qty as QtyBom,Unit_Cls FROM MS_BOMPerworkstation_Detail
					  ) C ON C.Bomws_ID=A.Bomws_ID
				WHERE A.ParentItemCode=@ParentItem
			  ) bom ON bom.Line_Code=scan.LineCode AND bom.WorkStationCode=scan.WorkStationCode AND bom.ChildItem_Code=scan.ItemCode 
		LEFT JOIN (SELECT BarcodeMaterial,MaterialCode,LotNo,SUM(QtyUsed) QtyUsed FROM MaterialConsumptionDetail GROUP BY BarcodeMaterial,MaterialCode,LotNo) cons ON cons.BarcodeMaterial=stock.BarcodeNo and cons.MaterialCode=stock.ItemCode and cons.LotNo=stock.LotNo
		LEFT JOIN Item_Master itm ON itm.Item_Code=scan.ItemCode
		WHERE COALESCE(stock.QtyStock,0) - COALESCE(QtyUsed,0) >0
		ORDER BY bom.Bomws_ID, scan.WorkStationCode,scan.StopPointCode,scan.RegisterDate

	OPEN ConsumptCursor 	

					
	FETCH NEXT FROM ConsumptCursor							
	INTO @LineNoConsumpt,@WorkStationCode,@StopPointCode, @ParentItemCodeConsumpt, @ItemCodeConsumpt,@ItemNameConsumpt,@QtyBom, @BarcodeNoConsumpt, @LotNoConsumpt, @Qty,@Used,@FromRefNo		
	WHILE ( @@FETCH_STATUS = 0 )							
	BEGIN	
			IF @ItemCodeBefore = @ItemCodeConsumpt
				BEGIN				
					SET @PackingQty = @QtySisa			
								
					SET @QtySisa = 0			
				END				
			ELSE					
			BEGIN				
				SET @PackingQty =  @QtyResult * @QtyBOM 
			END			
				
			SET @QtySisa = 0;	
								
			IF ((@Qty - @Used) - @PackingQty) < 0					
			BEGIN				
				SET @QtySisa = @PackingQty - (@Qty - @Used)			
				SET @PackingQty = @Qty - @Used			
			END			
			
			
			IF @PackingQty > 0					
			BEGIN	
				PRINT('CONSUMP')
				---consumption
				INSERT INTO MaterialConsumptionDetail(ResultDetailID,BarcodeMaterial,MaterialCode,MaterialName,LotNo,QtyUsed,RegisterUser,RegisterDate)
				VALUES (@ResultDetailID,@BarcodeNoConsumpt,@ItemCodeConsumpt,@ItemNameConsumpt,@LotNoConsumpt,@PackingQty,@UserID,GETDATE())

				--update stock detail
				UPDATE StockDetail
				SET Qty=Qty-@PackingQty,Lastupdate=GETDATE(),LastUser=@UserID
				WHERE COALESCE(Qty,0)>0 AND BarcodeNo=@BarcodeNoConsumpt AND ItemCode=@ItemCodeConsumpt AND LotNo=@LotNoConsumpt
				
				exec sp_Wms_Stock_UpSertStockHeader @Date, @FromRefNo,  @LineNoConsumpt, @WorkStationCode, @ItemCodeConsumpt, @LotNoConsumpt, @PackingQty, NULL, 'S', @UserId

				--update StockHeader_TRIALCONSUMPT 
				--set 
				--	TMCurrent	= ISNULL(TMCurrent, 0) - @PackingQty, 
				--	TMSupply	= ISNULL(TMSupply, 0) + @PackingQty, 
				--	TMInventory = NULL,
				--	NMPreMonth	= ISNULL(NMPreMonth, 0) - @PackingQty, 
				--	NMCurrent	= ISNULL(NMCurrent, 0) - @PackingQty,
				--	LastUpdate	= getdate(),
				--	LastUser	= @UserId
				--where RefNo = @FromRefNo and WarehouseCode =@LineNoConsumpt	 and AreaCode = @WorkStationCode and ItemCode = @ItemCodeConsumpt and LotNo = @LotNoConsumpt

			END 

			SET @ItemCodeBefore = @ItemCodeConsumpt					
			SET @Counter = @Counter + 1					
			
			FETCH NEXT FROM ConsumptCursor							
			INTO @LineNoConsumpt,@WorkStationCode,@StopPointCode, @ParentItemCodeConsumpt, @ItemCodeConsumpt,@ItemNameConsumpt,@QtyBom, @BarcodeNoConsumpt, @LotNoConsumpt, @Qty,@Used,@FromRefNo	

	END

					
	CLOSE ConsumptCursor 							
	DEALLOCATE ConsumptCursor 	

END


