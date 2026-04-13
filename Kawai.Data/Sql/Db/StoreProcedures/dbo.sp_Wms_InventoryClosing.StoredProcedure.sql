SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE   procedure [sp_Wms_InventoryClosing]
--DECLARE
	@Year		NUMERIC(4,0),
	@Month		NUMERIC(2,0),	
	@UserId		VARCHAR(30)
AS

 
DECLARE @ClosingDate AS DATETIME
SET @ClosingDate = (
					SELECT CAST (CAST(year AS VARCHAR(4)) + CASE WHEN month < 10 THEN '0' ELSE '' END 
					+ CAST (month AS VARCHAR(2)) + '01' AS DATETIME) ClosingDate
					FROM    ( SELECT TOP 1 MAX(inventory_month) month,inventory_year year
								FROM      inventory_control
								WHERE     fix_cls = '1' --and approve_cls = '1'
								GROUP BY  inventory_year
								ORDER BY  inventory_year DESC
							) tbA  
					)

BEGIN 

		/*INSERT STOCK HEADER HISTORY*/
		INSERT INTO StockHeader_History
		(
			[Period],RefNo,WarehouseCode,AreaCode,ItemCode,LotNo,LMPreMonth,LMReceipt,LMSupply,LMLossReject,LMCurrent,LMInventory,TMPreMonth,TMReceipt,TMSupply,TMLossReject,TMCurrent,TMInventory,NMPreMonth,NMReceipt,NMSupply,NMLossReject,NMCurrent,NMInventory,LMReason,TMReason,NMReason,Adjustment,LastUpdate,LastUser,RegisterDate
		)
		SELECT CONVERT(VARCHAR(6),@ClosingDate,112)[Period],RefNo,WarehouseCode,AreaCode,ItemCode,LotNo,LMPreMonth,LMReceipt,LMSupply,LMLossReject,LMCurrent,LMInventory,TMPreMonth,TMReceipt,TMSupply,TMLossReject,TMCurrent,TMInventory,NMPreMonth,NMReceipt,NMSupply,NMLossReject,NMCurrent,NMInventory,LMReason,TMReason,NMReason,Adjustment,LastUpdate,LastUser,RegisterDate
		FROM StockHeader

		/*INSERT STOCK DETAIL HISTORY*/
		INSERT INTO StockDetail_History
		(
			[Period],RefNo,WarehouseCode,AreaCode,AddressCode,BarcodeNo,ItemCode,LotNo,SublotNo,Qty,InventoryQty,ExpiredDate,ProductionDate,ReceiptDate,Supplier,PrintCls,DisposalCls,StatusReceipt,RegisterDate,RegisterUser,Lastupdate,LastUser,ClosingDate,ClosingUser
		)
		SELECT CONVERT(VARCHAR(6),@ClosingDate,112)[Period],RefNo,WarehouseCode,AreaCode,AddressCode,BarcodeNo,ItemCode,LotNo,SublotNo,Qty,InventoryQty,ExpiredDate,ProductionDate,ReceiptDate,Supplier,PrintCls,DisposalCls,StatusReceipt,RegisterDate,RegisterUser,Lastupdate,LastUser,GETDATE(),@UserId
		FROM [StockDetail]
END


BEGIN 
		/*UPDATE STOCK MASTER*/
		--TM MENJADI LM
		UPDATE StockHeader
		SET LMPreMonth = TMPreMonth,
			LMReceipt = TMReceipt,
			LMSupply = TMSupply,
			LMLossReject = TMLossReject,
			LMCurrent = TMCurrent,
			LMInventory = COALESCE(TMInventory,0) --Update stock berdasarkan actual, jika tidak ada (nilai nya null) di anggap 0
		
		--NM MENJADI TM
		UPDATE StockHeader
		SET TMPreMonth = LMInventory,
			TMReceipt = NMReceipt,
			TMSupply = NMSupply,
			TMLossReject = NMLossReject,						
			TMInventory = NULL 
		
		--FORMULA
		UPDATE dbo.StockHeader
		SET TMCurrent = (TMPreMonth + TMReceipt) - (TMSupply + TMLossReject)
		
					
		UPDATE StockHeader
		SET NMPreMonth = TMCurrent,
			NMReceipt = 0,
			NMSupply = 0,
			NMLossReject = 0,
			NMCurrent = TMCurrent,
			NMInventory = NULL 
	

		/*DELETE JIKA DATA KESELURUHAN 0*/
		DELETE StockHeader
		WHERE 
		COALESCE(LMPreMonth,0) + COALESCE(LMReceipt,0) + COALESCE(LMSupply,0) + COALESCE(LMLossReject,0) + COALESCE(LMCurrent,0) + COALESCE(LMInventory,0) +
		COALESCE(TMPreMonth,0) + COALESCE(TMReceipt,0) + COALESCE(TMSupply,0) + COALESCE(TMLossReject,0) + COALESCE(TMCurrent,0) + COALESCE(TMInventory,0) +
		COALESCE(NMPreMonth,0) + COALESCE(NMReceipt,0) + COALESCE(NMSupply,0) + COALESCE(NMLossReject,0) + COALESCE(NMCurrent,0) + COALESCE(NMInventory,0) <= 0 
		
		DELETE FROM StockHeader
		WHERE COALESCE(LMInventory,0)=0


		/*UPDATE STOCK DETAIL*/
		--UPDATE Qty dengan InventoryQty
		UPDATE [StockDetail]
		SET Qty = InventoryQty
		WHERE InventoryQty > 0 AND Qty > 0
	 

		--DELETE InventoryQty yang nilainya 0 atau NULL
		DELETE [StockDetail]
		WHERE coalesce(InventoryQty,0) = 0
		


		--DELETE Stock detail yang Qty 0 atau minus
		DELETE [StockDetail]
		WHERE Qty <= 0 
		

		--UPDATE InventoryQty menjadi NULL
		UPDATE [StockDetail]
		SET InventoryQty = NULL
	 

 END
 

GO
