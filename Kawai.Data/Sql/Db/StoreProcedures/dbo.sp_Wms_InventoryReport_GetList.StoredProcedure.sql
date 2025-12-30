CREATE OR ALTER PROCEDURE [dbo].[sp_Wms_InventoryReport_GetList]
--DECLARE
	@AreaCode		VARCHAR(50) = 'ALL',
	@WarehouseCode  VARCHAR(50) = 'ALL',
	@Period			DATE		= '2025-10-01'
AS
BEGIN
	
IF @Period IS NULL
BEGIN
	SET @Period = CAST(FORMAT(GETDATE(), 'yyyy-MM') + '-01' AS DATE)
END
	
BEGIN
	SELECT		
				--AreaCode = RTRIM(ISNULL(MA.AreaName,'')) ,
				Warehouse = RTRIM(ISNULL(MW.WarehouseName,'')) ,
                ProductCode = RTRIM(ISNULL(TBL.ItemCode,'')),
				ProductName = RTRIM(ISNULL(MI.Item_Name,'')),
				LotNo = RTRIM(ISNULL(TBL.Lot_No,'')) ,
				[PreMonth] = SUM(ISNULL(TBL.PreMonth,0)) ,
				[Receipt] = SUM(ISNULL(TBL.Receipt,0)) ,
				[Supply] = SUM(ISNULL(TBL.Supply,0)) ,
				[LossReject] = SUM(ISNULL(TBL.LossReject,0)) ,
				[Current] = SUM(ISNULL(TBL.[Current],0)) ,
                [Inventory] = SUM(ISNULL(TBL.[Inventory],0)),
				[Remarks] = (ISNULL(TBL.Reason,'')),
				--LastUpdate = TBL.LastUpdate,
				LastUser = ISNULL(US.FullName,'')
        FROM    ( 
				 /* /*Belum ada StockHistory*/
				 SELECT     Stock_Year ,
                            Stock_Month ,
                            AreaCode,
                            WarehouseCode ,
                            ItemCode,
                            Lot_No = LotNo ,
                            PreMonth ,
                            Receipt ,
                            Supply ,
                            LossReject ,
                            [Current] ,
                            Inventory = ISNULL(Inventory, [Current]) ,
							Differences = CASE WHEN  ISNULL(CAST(Inventory AS VARCHAR(100)),'') = '' THEN NULL 
											   WHEN  ISNULL(CAST(Inventory AS VARCHAR(100)),'') <> '' THEN ISNULL(Inventory, [Current]) - [Current] END,
                            Reason,
							LastUpdate = COALESCE(LastUpdate,RegisterDate,NULL),
							LastUser = ISNULL(LastUser,''),
							Conditon = 'SH'
                  FROM      dbo.StockHistory WITH (NOLOCK)
                  UNION ALL
				  */
                  SELECT    Stock_Year = YEAR(IC.Period) ,
                            Stock_Month = MONTH(IC.Period) ,
							AreaCode = SM.AreaCode,
                            Warehouse_Code = SM.WarehouseCode ,
                            ItemCode = ISNULL(SM.ItemCode,''),
                            Lot_No = SM.LotNo ,
                            PreMonth = SM.LMPreMonth ,
                            Receipt = SM.LMReceipt ,
                            Supply = SM.LMSupply ,
                            LossReject = SM.LMLossReject ,
                            [Current] = SM.LMCurrent ,
                            Inventory = SM.LMInventory ,
							Differences = CASE WHEN  ISNULL(CAST(SM.LMInventory AS VARCHAR(100)),'') = '' THEN NULL 
											   WHEN  ISNULL(CAST(SM.LMInventory AS VARCHAR(100)),'') <> '' THEN ISNULL(SM.LMInventory, SM.LMCurrent) - SM.LMCurrent END,
                            Reason = SM.LMReason,
							LastUpdate = COALESCE(SM.LastUpdate,SM.RegisterDate,NULL),
							LastUser = ISNULL(SM.LastUser,''),
							Conditon = 'LM'
                  FROM      dbo.StockHeader SM WITH (NOLOCK)
                            CROSS JOIN ( SELECT *
                                         FROM   ( SELECT TOP 1
                                                            CAST(RTRIM(Inventory_Year)
                                                            + '-'
                                                            + RTRIM(Inventory_Month)
                                                            + '-01' AS DATETIME) Period
                                                  FROM      dbo.Inventory_Control /*Belum ada StockOpname*/
                                                  ORDER BY  CAST(RTRIM(Inventory_Year)
                                                            + '-'
                                                            + RTRIM(Inventory_Month)
                                                            + '-01' AS DATETIME) DESC
                                                ) A
                                       ) IC
                  UNION ALL
                  SELECT    Stock_Year = YEAR(DATEADD(MONTH, 1, IC.Period)) ,
                            Stock_Month = MONTH(DATEADD(MONTH, 1, IC.Period)) , 
							AreaCode = SM.AreaCode,
                            Warehouse_Code = SM.WarehouseCode ,
                            ItemCode = ISNULL(SM.ItemCode,''),
                            Lot_No = SM.LotNo ,
                            PreMonth = SM.TMPreMonth ,
                            Receipt = SM.TMReceipt ,
                            Supply = SM.TMSupply ,
                            LossReject = SM.TMLossReject ,
                            [Current] = SM.TMCurrent ,
                            Inventory = SM.TMInventory ,
							Differences = CASE WHEN  ISNULL(CAST(SM.TMInventory AS VARCHAR(100)),'') = '' THEN NULL 
											   WHEN  ISNULL(CAST(SM.TMInventory AS VARCHAR(100)),'') <> '' THEN ISNULL(SM.TMInventory, SM.TMCurrent) - SM.TMCurrent END,
                            Reason = SM.TMReason,
							LastUpdate = COALESCE(SM.LastUpdate,SM.RegisterDate,NULL),
							LastUser = ISNULL(SM.LastUser,''),
							Conditon = 'TM'
                  FROM      dbo.StockHeader SM WITH (NOLOCK)
                            CROSS JOIN ( SELECT *
                                         FROM   ( SELECT TOP 1
                                                            CAST(RTRIM(Inventory_Year)
                                                            + '-'
                                                            + RTRIM(Inventory_Month)
                                                            + '-01' AS DATETIME) Period
                                                  FROM      dbo.Inventory_Control /*Belum ada StockOpname*/
                                                  ORDER BY  CAST(RTRIM(Inventory_Year)
                                                            + '-'
                                                            + RTRIM(Inventory_Month)
                                                            + '-01' AS DATETIME) DESC
                                                ) A
                                       ) IC
                  UNION ALL
                  SELECT    Stock_Year = YEAR(DATEADD(MONTH, 2, IC.Period)) ,
                            Stock_Month = MONTH(DATEADD(MONTH, 2, IC.Period)) ,
							AreaCode = SM.AreaCode,
                            Warehouse_Code = SM.WarehouseCode ,
                            ItemCode = ISNULL(SM.ItemCode,''),
                            Lot_No = SM.LotNo ,
                            PreMonth = SM.NMPreMonth ,
                            Receipt = SM.NMReceipt ,
                            Supply = SM.NMSupply ,
                            LossReject = SM.NMLossReject ,
                            [Current] = SM.NMCurrent ,
                            Inventory = SM.NMInventory ,
							Differences = CASE WHEN  ISNULL(CAST(SM.NMInventory AS VARCHAR(100)),'') = '' THEN NULL 
											   WHEN  ISNULL(CAST(SM.NMInventory AS VARCHAR(100)),'') <> '' THEN ISNULL(SM.NMInventory, SM.NMCurrent) - SM.NMCurrent END,
                            Reason = SM.NMReason,
							LastUpdate = COALESCE(SM.LastUpdate,SM.RegisterDate,NULL),
							LastUser = ISNULL(SM.LastUser,''),
							Conditon = 'NM'
                  FROM      dbo.StockHeader SM WITH (NOLOCK)
                            CROSS JOIN ( SELECT *
                                         FROM   ( SELECT TOP 1
                                                            CAST(RTRIM(Inventory_Year)
                                                            + '-'
                                                            + RTRIM(Inventory_Month)
                                                            + '-01' AS DATETIME) Period
                                                  FROM      dbo.Inventory_Control /*Belum ada StockOpname*/
                                                  ORDER BY  CAST(RTRIM(Inventory_Year)
                                                            + '-'
                                                            + RTRIM(Inventory_Month)
                                                            + '-01' AS DATETIME) DESC
                                                ) A
                                       ) IC
                ) TBL
                LEFT JOIN dbo.Item_Master MI WITH (NOLOCK) ON MI.Item_Code = TBL.ItemCode
                LEFT JOIN dbo.vw_WarehouseLine MW WITH (NOLOCK) ON MW.WarehouseCode = TBL.Warehouse_Code
				LEFT JOIN dbo.vw_Area MA WITH (NOLOCK) ON MA.AreaCode = TBL.AreaCode
				LEFT JOIN dbo.SS_UserSetup US WITH (NOLOCK) ON US.UserID = TBL.LastUser
				WHERE TBL.[Stock_Year] = YEAR(@Period) AND TBL.[Stock_Month] = MONTH(@Period)
				AND 1 = CASE WHEN @AreaCode = '' OR @AreaCode = 'ALL' THEN 1
							 WHEN @AreaCode <> 'ALL' AND @AreaCode <> '' AND TBL.AreaCode = @AreaCode THEN 1
							 ELSE 0
						END
				AND 1 = CASE WHEN @WarehouseCode = '' OR @WarehouseCode = 'ALL' THEN 1
							 WHEN @WarehouseCode <> 'ALL' AND @WarehouseCode <> '' AND TBL.Warehouse_Code = @WarehouseCode THEN 1
							 ELSE 0
						END
				
				GROUP BY 
				--RTRIM(ISNULL(MA.AreaName,'')) ,
				RTRIM(ISNULL(MW.WarehouseName,'')) ,
                RTRIM(ISNULL(TBL.ItemCode,'')),
				RTRIM(ISNULL(MI.Item_Name,'')),
				RTRIM(ISNULL(TBL.Lot_No,'')) ,
				(ISNULL(TBL.Reason,'')),
				--TBL.LastUpdate,
				ISNULL(US.FullName,'')
END

END
GO


