
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
	exec [sp_Wms_Physical_Inventory_List] 1,10,'ItemCode asc','', 
	'2025-07-01','WH-001','WH-001/001','WH-001/WH-001/001/001','','','ALL','false'
*/
ALTER         procedure [dbo].[sp_Wms_Physical_Inventory_List]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@Period date = null,
	@WarehouseCode varchar(25) = null,
	@AreaCode varchar(25) = null,
	@AddressCode varchar(25) = null,
	@ItemCode varchar(25) = null,
	@LotNo varchar(25) = null,
	@ScanStatus varchar(25) = null,
	@DifferentQty varchar(25) = null
as
begin
		
    -- Hitung offset berdasarkan halaman
	declare @offset int
    set @offset = (@Page - 1) * @Length
	
	PRINT @offset

	declare @TotalRow int = 
	(
		SELECT COUNT(1) AS TotalRow
		FROM StockDetail sd
		LEFT JOIN item_master im
			ON sd.ItemCode = im.item_code
		LEFT JOIN unit_cls uc
			ON uc.unit_cls = im.unit_cls
		LEFT JOIN StockOpname so 
			ON sd.BarcodeNo = so.BarcodeNo
		WHERE year(sd.ProductionDate) = year(@Period)
		AND	month(sd.ProductionDate) = MONTH(@Period)
		AND (
				@WarehouseCode IS NULL 
				OR @WarehouseCode = '' 
				OR sd.WarehouseCode = @WarehouseCode
			)
		AND (
				@AreaCode IS NULL 
				OR @AreaCode = '' 
				OR sd.AreaCode = @AreaCode
			)
		AND (
				@AddressCode IS NULL 
				OR @AddressCode = '' 
				OR sd.AddressCode = @AddressCode
			)
		AND (
				@ItemCode IS NULL 
				OR @ItemCode = '' 
				OR sd.ItemCode = @ItemCode
			)
		AND (
				@LotNo IS NULL 
				OR @LotNo = '' 
				OR sd.LotNo = @LotNo
			)
		AND (
				@ScanStatus IS NULL
				 OR  @ScanStatus = ''
				 OR  @ScanStatus = 'ALL'
				 OR (@ScanStatus = 'NOTYET' AND so.InventoryQty IS NULL)
				 OR (@ScanStatus = 'SCANNED' AND so.InventoryQty IS NOT NULL AND so.InventoryQty = sd.Qty)
				 OR (@ScanStatus = 'DIFFERENT' AND so.InventoryQty IS NOT NULL AND so.InventoryQty <> sd.Qty)
			)
		AND (
				ISNULL(@DifferentQty, 'false') <> 'true'
				 OR (so.InventoryQty IS NOT NULL AND so.InventoryQty <> sd.Qty)
			)
		AND (
				@Keyword IS NULL 
				OR @Keyword = '' 
				OR im.item_name	LIKE '%' + @Keyword + '%' 
				OR uc.description	LIKE '%' + @Keyword + '%' 
			)
	)
		

	SELECT
		TotalRows		= @TotalRow,
		RefNo			= sd.RefNo,
		WarehouseCode	= sd.WarehouseCode,
		WarehouseName	= wm.WH_Name,
		AreaCode		= sd.AreaCode,
		AreaName		= mar.AreaName,
		AddressCode		= isnull(sd.AddressCode,'-'),
		AddressName		= isnull(ma.AddressName,'-'),
		BarcodeNo		= isnull(sd.BarcodeNo, '-'),
		ItemCode		= sd.ItemCode,
		ItemDesc		= im.item_name,
		UnitCls			= im.unit_cls,
		Unit			= uc.description,
		LotNo			= sd.LotNo,
		CurrentQty		= isnull(sd.Qty,0),
		Inventory		= isnull(so.InventoryQty,0),
		StatusScan		= case when so.InventoryQty is null then 'NOTYET' when so.InventoryQty = sd.Qty then 'SCANNED' else 'DIFFERENT' end,
		LastUpdate		= so.LastUpdate,
		LastUserID	= isnull(so.LastUser,'-'),
		LastUserName	= us.FullName
	FROM StockDetail sd
	LEFT JOIN item_master im
		ON sd.ItemCode = im.item_code
	LEFT JOIN unit_cls uc
		ON uc.unit_cls = im.unit_cls
	LEFT JOIN StockOpname so 
		ON sd.BarcodeNo = so.BarcodeNo	
	LEFT JOIN WareHouse_Master wm 
		ON sd.WarehouseCode = wm.WH_Code
	LEFT JOIN MS_Area mar
		ON sd.AreaCode = mar.AreaCode
	LEFT JOIN MS_Address ma 
		ON sd.AddressCode = ma.AddressCode
	LEFT JOIN SS_UserSetup us
		ON so.LastUser = us.UserID
	WHERE year(sd.ProductionDate) = year(@Period)
	AND	month(sd.ProductionDate) = MONTH(@Period)
	AND	(
			@WarehouseCode IS NULL 
			OR @WarehouseCode = '' 
			OR sd.WarehouseCode = @WarehouseCode
		)
	AND (
			@AreaCode IS NULL 
			OR @AreaCode = '' 
			OR sd.AreaCode = @AreaCode
		)
	AND (
			@AddressCode IS NULL 
			OR @AddressCode = '' 
			OR sd.AddressCode = @AddressCode
		)
	AND (
			@ItemCode IS NULL 
			OR @ItemCode = '' 
			OR sd.ItemCode = @ItemCode
		)
	AND (
			@LotNo IS NULL 
			OR @LotNo = '' 
			OR sd.LotNo = @LotNo
		)
	AND (
			@ScanStatus IS NULL
			 OR  @ScanStatus = ''
			 OR  @ScanStatus = 'ALL'
			 OR (@ScanStatus = 'NOTYET' AND so.InventoryQty IS NULL)
			 OR (@ScanStatus = 'SCANNED' AND so.InventoryQty IS NOT NULL AND so.InventoryQty = sd.Qty)
			 OR (@ScanStatus = 'DIFFERENT' AND so.InventoryQty IS NOT NULL AND so.InventoryQty <> sd.Qty)
		)
	AND (
			ISNULL(@DifferentQty, 'false') <> 'true'
			 OR (so.InventoryQty IS NOT NULL AND so.InventoryQty <> sd.Qty)
		)
	AND (
			@Keyword IS NULL 
			OR @Keyword = '' 
			OR im.item_name	LIKE '%' + @Keyword + '%' 
			OR uc.description	LIKE '%' + @Keyword + '%' 		
		)

	ORDER BY 
		CASE WHEN RIGHT(LOWER(@Sort), 3) = 'asc'  THEN sd.ItemCode END ASC,
		CASE WHEN RIGHT(LOWER(@Sort), 4) = 'desc' THEN sd.ItemCode END DESC
	OFFSET @Offset ROWS
	FETCH NEXT @Length ROWS ONLY;
	--WHERE sm.warehouse_code = '888'
	--ORDER BY SM.Item_Code ASC
end