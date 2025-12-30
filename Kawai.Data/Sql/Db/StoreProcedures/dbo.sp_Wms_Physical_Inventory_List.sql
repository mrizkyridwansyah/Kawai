SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--exec [sp_Wms_Physical_Inventory_List] 1,10,'','', 'WH-001','','2025-12-01'
create or ALTER   procedure [dbo].[sp_Wms_Physical_Inventory_List]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@WarehouseCode varchar(25) = null,
	@ItemCode varchar(25) = null,
	@Period date = null
as
begin
	
	--VALIDASI PERIOD
	declare @dateRange int

	select @dateRange = dbo.fn_Wms_GetDateRange(@Period) 
	--select @dateRange = dbo.fn_Wms_GetDateRange('2025-12-01')

	if(@dateRange = -1)
	begin 
		raiserror('Period tidak sesuai!!!',16,1)
		return
	end
	
	PRINT @dateRange
	---

    -- Hitung offset berdasarkan halaman
	declare @offset int
    set @offset = (@Page - 1) * @Length
	
	PRINT @offset


	declare @TotalRow int = 
	(
		SELECT COUNT(1) AS TotalRow
		FROM stock_master sm
		INNER JOIN item_master im
			ON sm.item_code = im.item_code
		LEFT JOIN unit_cls uc
			ON uc.unit_cls = im.unit_cls
		WHERE  (
				@WarehouseCode IS NULL 
				OR @WarehouseCode = '' 
				OR sm.warehouse_code = @WarehouseCode
			)
		AND (
				@ItemCode IS NULL 
				OR @ItemCode = '' 
				OR sm.Item_Code = @ItemCode
			)
		AND (
				@Keyword IS NULL 
				 OR sm.item_code	LIKE '%' + @Keyword + '%' 
				 OR im.item_name	LIKE '%' + @Keyword + '%' 
				 OR uc.description	LIKE '%' + @Keyword + '%' 
				 OR address			LIKE '%' + @Keyword + '%' 
			)
	)
		

	SELECT
		TotalRows		= @TotalRow,
		WarehouseCode	= sm.warehouse_code,
		ProductCode		= sm.item_code,
		MakerItemCode	= im.makeritem_code,
		ProductDesc		= im.item_name,
		UnitCls			= im.unit_cls,
		Unit			= uc.description,
		Address			= isnull(address,'-'),
		PreMonthStock	= case @dateRange 
								when 0 then LM_PreMonth
								when 1 then TM_PreMonth
								when 2 then NM_PreMonth
							else '-' end,
		ReceiptTotal	= case @dateRange 
								when 0 then LM_Receipt
								when 1 then TM_Receipt
								when 2 then NM_Receipt
							else '-' end,
		SupplyTotal		= case @dateRange 
								when 0 then LM_Supply
								when 1 then TM_Supply
								when 2 then NM_Supply
							else '-' end,
		Loss			= case @dateRange 
								when 0 then LM_LossReject
								when 1 then TM_LossReject
								when 2 then NM_LossReject
							else '-' end,
		EndOfMonthStock = case @dateRange 
								when 0 then LM_Current
								when 1 then TM_Current
								when 2 then NM_Current
							else '-' end,
		Inventory		= case @dateRange 
								when 0 then isnull(LM_Inventory,0) 
								when 1 then isnull(TM_Inventory,0)
								when 2 then isnull(NM_Inventory,0)
							else '-' end,
		Inventory2		= case @dateRange 
								when 0 then isnull(LM_Inventory,0) 
								when 1 then isnull(TM_Inventory,0)
								when 2 then isnull(NM_Inventory,0)
							else '-' end,
		Differences		= case @dateRange
								when 0 then isnull(LM_Inventory, 0) - isnull(LM_Current, 0)
								when 1 then isnull(TM_Inventory, 0) - isnull(TM_Current, 0)
								when 2 then isnull(NM_Inventory, 0) - isnull(NM_Current, 0)
								else 0
							end,
		Reason			= case @dateRange
								when 0 then isnull(LM_Reason, '') 
								when 1 then isnull(TM_Reason, '') 
								when 2 then isnull(NM_Reason, '') 
								else ''
							end,
		Reason2			= case @dateRange
								when 0 then isnull(LM_Reason, '') 
								when 1 then isnull(TM_Reason, '') 
								when 2 then isnull(NM_Reason, '') 
								else ''
							end
		--g di pake
		--sheetcoil_cls,
		--width,
		--length,
		--thickness,
	FROM stock_master sm
	INNER JOIN item_master im
		ON sm.item_code = im.item_code
	LEFT JOIN unit_cls uc
		ON uc.unit_cls = im.unit_cls
	WHERE  (
			@WarehouseCode IS NULL 
			OR @WarehouseCode = '' 
			OR sm.warehouse_code = @WarehouseCode
		)
	AND (
			@ItemCode IS NULL 
			OR @ItemCode = '' 
			OR sm.Item_Code = @ItemCode
		)
	AND (
			@Keyword IS NULL 
			 OR sm.item_code	LIKE '%' + @Keyword + '%' 
			 OR im.item_name	LIKE '%' + @Keyword + '%' 
			 OR uc.description	LIKE '%' + @Keyword + '%' 
			 OR address			LIKE '%' + @Keyword + '%' 
		)
	ORDER BY 
		CASE WHEN RIGHT(LOWER(@Sort), 3) = 'asc'  THEN SM.Item_Code END ASC,
		CASE WHEN RIGHT(LOWER(@Sort), 4) = 'desc' THEN SM.Item_Code END DESC
	OFFSET @Offset ROWS
	FETCH NEXT @Length ROWS ONLY;
	--WHERE sm.warehouse_code = '888'
	--ORDER BY SM.Item_Code ASC
end