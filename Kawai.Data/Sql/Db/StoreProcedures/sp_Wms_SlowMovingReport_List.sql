--EXEC sp_Wms_SlowMovingReport_List 1,10,'','','00000','WH-001','2025-06-01'
create or alter procedure sp_Wms_SlowMovingReport_List
(
	-- PARAMETER WAJIB
	@Page			int = 1,
	@Length			int = 10,
	@Sort			varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword		NVARCHAR(max) = '',
	@FactoryCode	NVARCHAR(25),
	@WarehouseCode	NVARCHAR(25),
	@Period			DATETIME
)
as
begin	

	SET NOCOUNT ON;

	/* =====================================================
       1. SETUP PERIODE
    ===================================================== */
    DECLARE @LastMonth INT;
    SET @LastMonth = YEAR(@Period) * 100 + MONTH(@Period);


	/* =====================================================
       2. BUILD DYNAMIC PIVOT FIELD (7 BULAN KE BELAKANG)
    ===================================================== */
    DECLARE @FieldName NVARCHAR(MAX) = '';

    SELECT @FieldName = STRING_AGG(QUOTENAME(Periode), ',')
    FROM (
        SELECT CONVERT(VARCHAR(6), DATEADD(MONTH, -v.n, @Period), 112) AS Periode
        FROM (VALUES (0),(1),(2),(3),(4),(5),(6)) v(n)
    ) x;

	--select @FieldName as FieldName

	/* =====================================================
       3. CALCULATE OFFSET AND MAKE SORT BY
    ===================================================== */
	declare @offset int
    set @offset = (@Page - 1) * @Length

	DECLARE @OrderBy NVARCHAR(200);

	SET @OrderBy =
		CASE @Sort
			WHEN 'ItemCode asc' THEN 'Item_Code ASC'
			WHEN 'ItemCode desc' THEN 'Item_Code DESC'
			WHEN 'WarehouseCode asc' THEN 'WHCode ASC'
			WHEN 'WarehouseCode desc' THEN 'WHCode DESC'
			WHEN 'LongStock asc' THEN 'LongStock ASC'
			WHEN 'LongStock desc' THEN 'LongStock DESC'
			ELSE 'LongStock, Item_Code'
		END;


	/* =====================================================
       4. DYNAMIC QUERY
    ===================================================== */
    DECLARE @SQL NVARCHAR(MAX);

	SET @SQL = N'
    WITH StockBase AS (
        SELECT
			SH.Reason,
            SH.Warehouse_Code,
            SH.Item_Code,
            SH.Stock_Year * 100 + SH.Stock_Month AS Period,
            COALESCE(SH.Inventory, SH.[Current]) AS Qty
        FROM Stock_History SH
        WHERE SH.Stock_Year * 100 + SH.Stock_Month <= @LastMonth
			 AND SH.Warehouse_Code IN (
				SELECT WH_Code FROM WareHouse_Master 
				WHERE Company_Code = CASE WHEN @FactoryCode = '''' THEN Company_Code ELSE @FactoryCode END
				AND WH_Code = CASE WHEN @WarehouseCode = '''' THEN WH_Code ELSE @WarehouseCode END
			)           
    ),
    LastMovement AS (
        SELECT
            Item_Code,
            MAX(MoveDate) AS LastMoveDate
        FROM (
            SELECT Item_Code, Receipt_Date AS MoveDate FROM Part_Receipt
            UNION ALL
            SELECT ChildItem_Code AS Item_Code, ChildSupply_date AS MoveDate FROM Part_Supply
        ) M
        GROUP BY Item_Code
    ),
    ItemCategory AS (
        SELECT
            LM.Item_Code,
            CASE
                WHEN DATEDIFF(MONTH, LM.LastMoveDate, @Period) > 7 THEN ''Red Card''
                WHEN DATEDIFF(MONTH, LM.LastMoveDate, @Period) BETWEEN 4 AND 6 THEN ''Yellow Card''
                WHEN DATEDIFF(MONTH, LM.LastMoveDate, @Period) BETWEEN 2 AND 3 THEN ''White Card''
                ELSE NULL
            END AS LongStock
        FROM LastMovement LM
    ),
    FinalData AS (
        SELECT
            IC.LongStock,
            SB.Item_Code,
            I.Item_Name,
            U.Description AS Unit_Name,
            SB.Warehouse_Code AS WHCode,
            W.WH_Name AS WHName,
            SB.Period,
            SB.Qty,
			SB.Reason AS Remarks
        FROM StockBase SB
        JOIN ItemCategory IC ON SB.Item_Code = IC.Item_Code
        JOIN Item_Master I ON SB.Item_Code = I.Item_Code
        JOIN Unit_Cls U ON I.Unit_Cls = U.Unit_Cls
        JOIN WareHouse_Master W ON W.WH_Code = SB.Warehouse_Code
        WHERE IC.LongStock IS NOT NULL
		AND (
			@Keyword = ''''
			OR SB.Item_Code LIKE ''%'' + @Keyword + ''%''
			OR I.Item_Name LIKE ''%'' + @Keyword + ''%''
			OR SB.Warehouse_Code LIKE ''%'' + @Keyword + ''%''
			OR W.WH_Name LIKE ''%'' + @Keyword + ''%''
		)
    )
    SELECT
		COUNT(*) OVER() AS TotalRows,
        LongStock,
        Item_Code,
        Item_Name,
        Unit_Name,
        WHCode,
        WHName,
		Remarks,
        ' + @FieldName + '
    FROM FinalData
    PIVOT (
        SUM(Qty)
        FOR Period IN (' + @FieldName + ')
    ) P
    ORDER BY ' + @OrderBy + '
	OFFSET @Offset ROWS
	FETCH NEXT @Length ROWS ONLY;
    ';

    /* =====================================================
       4. EXECUTE
    ===================================================== */
    EXEC sp_executesql
        @SQL,
        N'@FactoryCode NVARCHAR(25), @WarehouseCode NVARCHAR(25), @Period DATE, @LastMonth INT, @Keyword NVARCHAR(MAX), @offset INT, @Length INT ',
        @FactoryCode = @FactoryCode,
        @WarehouseCode = @WarehouseCode,
        @Period = @Period,
        @LastMonth = @LastMonth,
        @Keyword = @Keyword,
        @offset = @offset,
        @Length = @Length;

end