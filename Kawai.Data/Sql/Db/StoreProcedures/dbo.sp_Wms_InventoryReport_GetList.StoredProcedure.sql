SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Wms_InventoryReport_GetList]
--DECLARE
    @Page           INT = 1,
    @Length         INT = 10,
    @Sort           VARCHAR(MAX) = '',
	-- PARAMETER OPSIONAL
    @Keyword        VARCHAR(MAX) = '',
    @WarehouseCode  VARCHAR(50) = '',
    @AreaCode       VARCHAR(50) = '',
    @Period         DATE = '2025-12-01'
AS
BEGIN
    SET NOCOUNT ON;

    IF @Period IS NULL
        SET @Period = CAST(FORMAT(GETDATE(), 'yyyy-MM') + '-01' AS DATE);

    DECLARE @Offset INT = (@Page - 1) * @Length;
    DECLARE @SqlSort VARCHAR(MAX);
    DECLARE @Sql NVARCHAR(MAX);
    DECLARE @TotalRow INT;

    IF ISNULL(@Sort, '') <> ''
        SET @SqlSort = ' ORDER BY ' + @Sort;
    ELSE
        SET @SqlSort = ' ORDER BY RTRIM(ISNULL(TBL.Warehouse, '''')), RTRIM(ISNULL(TBL.ProductCode, '''')), RTRIM(ISNULL(TBL.LotNo, ''''))';

    /* =========================
       HITUNG TOTAL ROW
    ==========================*/
    SELECT @TotalRow = COUNT(1)
    FROM (
        SELECT		
				--AreaCode = RTRIM(ISNULL(MA.AreaName,'')) ,
				Warehouse = RTRIM(ISNULL(TBL.Warehouse,'')) ,
                ProductCode = RTRIM(ISNULL(TBL.ProductCode,'')),
				ProductName = RTRIM(ISNULL(TBL.ProductName,'')),
				LotNo = RTRIM(ISNULL(TBL.LotNo,'')) ,
				[PreMonth] = (ISNULL(TBL.PreMonth,0)) ,
				[Receipt] = (ISNULL(TBL.Receipt,0)) ,
				[Supply] = (ISNULL(TBL.Supply,0)) ,
				[LossReject] = (ISNULL(TBL.LossReject,0)) ,
				[Current] = (ISNULL(TBL.[Current],0)) ,
                [Inventory] = (ISNULL(TBL.[Inventory],0)),
				[Remarks] = (ISNULL(TBL.Remarks,'')),
				--LastUpdate = TBL.LastUpdate,
				LastUser = ISNULL(TBL.LastUser,'')
        FROM    dbo.vw_StockHeader TBL
				WHERE TBL.Stock_Year = YEAR(@Period) AND TBL.Stock_Month = MONTH(@Period)
				AND 1 = CASE WHEN @AreaCode = '' OR @AreaCode = 'ALL' THEN 0
							 WHEN @AreaCode <> 'ALL' AND @AreaCode <> '' AND TBL.AreaCode = @AreaCode THEN 1
							 ELSE 0
						END
				AND 1 = CASE WHEN @WarehouseCode = '' OR @WarehouseCode = 'ALL' THEN 0
							 WHEN @WarehouseCode <> 'ALL' AND @WarehouseCode <> '' AND TBL.WarehouseCode = @WarehouseCode THEN 1
							 ELSE 0
						END
				 AND (
						@Keyword = ''
						OR TBL.ProductCode LIKE '%' + @Keyword + '%'
						OR TBL.ProductName LIKE '%' + @Keyword + '%'
						OR TBL.Warehouse   LIKE '%' + @Keyword + '%'
						OR TBL.LotNo       LIKE '%' + @Keyword + '%'
					 )
    ) X;

    /* =========================
       DATA + PAGING
    ==========================*/
    SET @Sql =
N'SELECT
    Warehouse   = RTRIM(ISNULL(TBL.Warehouse, '''')),
    ProductCode = RTRIM(ISNULL(TBL.ProductCode, '''')),
    ProductName = RTRIM(ISNULL(TBL.ProductName, '''')),
    LotNo       = RTRIM(ISNULL(TBL.LotNo, '''')),
    [PreMonth]  = (ISNULL(TBL.PreMonth, 0)),
    [Receipt]   = (ISNULL(TBL.Receipt, 0)),
    [Supply]    = (ISNULL(TBL.Supply, 0)),
    [LossReject]= (ISNULL(TBL.LossReject, 0)),
    [Current]   = (ISNULL(TBL.[Current], 0)),
    [Inventory] = (ISNULL(TBL.[Inventory], 0)),
    [Remarks]   = ISNULL(TBL.Remarks, ''''),
    LastUser    = ISNULL(TBL.LastUser, ''''),
	[TotalRows] = '''+CAST(@TotalRow AS VARCHAR)+'''
FROM dbo.vw_StockHeader TBL
WHERE TBL.Stock_Year  = YEAR(@Period) AND TBL.Stock_Month = MONTH(@Period)
    AND 1 = CASE
        WHEN @AreaCode = '''' OR @AreaCode = ''ALL'' THEN 0
        WHEN TBL.AreaCode = @AreaCode THEN 1
        ELSE 0
    END
    AND 1 = CASE
        WHEN @WarehouseCode = '''' OR @WarehouseCode = ''ALL'' THEN 0
        WHEN TBL.WarehouseCode = @WarehouseCode THEN 1
        ELSE 0
    END
	AND (
            @Keyword = ''''
            OR TBL.ProductCode LIKE ''%'' + @Keyword + ''%''
            OR TBL.ProductName LIKE ''%'' + @Keyword + ''%''
            OR TBL.Warehouse   LIKE ''%'' + @Keyword + ''%''
            OR TBL.LotNo       LIKE ''%'' + @Keyword + ''%''
        )
' + @SqlSort + '
    OFFSET @Offset ROWS FETCH NEXT @Length ROWS ONLY';


	--PRINT @Sql
    EXEC sp_executesql
        @Sql,
        N'@Keyword VARCHAR(MAX), @WarehouseCode VARCHAR(50), @AreaCode VARCHAR(50),
          @Period DATE, @Offset INT, @Length INT, @TotalRow INT',
        @Keyword,
        @WarehouseCode,
        @AreaCode,
        @Period,
        @Offset,
        @Length,
        @TotalRow;
END
GO
