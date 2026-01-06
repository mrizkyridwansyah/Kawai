SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[sp_Wms_Expired_GetList]
(
    @Page INT = 1,
    @Length INT = 5,
    @Sort NVARCHAR(MAX) = NULL,

    @Keyword varchar(max) = '',
	@ExpiredUntil DATE = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @Filters NVARCHAR(MAX) = NULL

    ----------------------------------------------------------------
    -- 1?? SIMULASI DATA (DUMMY DATA)
    ----------------------------------------------------------------
    CREATE TABLE #Data
    (
        WHCode NVARCHAR(50),
        WHName NVARCHAR(100),
        ItemCode NVARCHAR(50),
        PartNo NVARCHAR(50),
        ItemName NVARCHAR(100),
        Unit NVARCHAR(20),
        LotNo NVARCHAR(20),
        ReceiptDate DATE,
        ManufactureDate DATE,
        ExpiredDay DECIMAL(18,2),
        Qty DECIMAL(18,2),
        ExpiredDate DATE,
        Status NVARCHAR(20)
    );

    INSERT INTO #Data VALUES
    ('WH-01-100', 'Gudang RAW', '5Z5681', 'Material', 'KARTON TOP', 'Pcs', '0882-0004', '2025-02-20', '2025-02-20', 1, 0, @ExpiredUntil, 'EXPIRED'),
    ('WH-01-100', 'Gudang RAW', '5Z5681', 'Material', 'KARTON TOP', 'Pcs', '0882-0004', '2025-03-01', '2025-03-01', 2, 0, @ExpiredUntil, 'EXPIRED'),
    ('WH-01-100', 'Gudang RAW', '5Z5681', 'Material', 'KARTON TOP', 'Pcs', '0882-0004', '2025-01-15', '2025-01-15', 3, 0, @ExpiredUntil, 'EXPIRED'),
    ('WH-01-100', 'Gudang RAW', '5Z5681', 'Material', 'KARTON TOP', 'Pcs', '0882-0004', '2025-04-10', '2025-04-10', 4, 0, @ExpiredUntil, 'EXPIRED'),
    ('WH-01-100', 'Gudang RAW', '5Z5681', 'Material', 'KARTON TOP', 'Pcs', '0882-0004', '2025-02-28', '2025-02-28', 5, 0, @ExpiredUntil, 'EXPIRED'),
    ('WH-01-200', 'Gudang RAW', '5Z5682', 'Material', 'KARTON TOP', 'Pcs', '0882-0005', '2025-02-20', '2025-02-20', 1, 0, @ExpiredUntil, 'EXPIRED'),
    ('WH-01-200', 'Gudang RAW', '5Z5682', 'Material', 'KARTON TOP', 'Pcs', '0882-0005', '2025-03-01', '2025-03-01', 2, 0, @ExpiredUntil, 'EXPIRED'),
    ('WH-01-200', 'Gudang RAW', '5Z5682', 'Material', 'KARTON TOP', 'Pcs', '0882-0005', '2025-01-15', '2025-01-15', 3, 0, @ExpiredUntil, 'EXPIRED'),
    ('WH-01-200', 'Gudang RAW', '5Z5682', 'Material', 'KARTON TOP', 'Pcs', '0882-0005', '2025-04-10', '2025-04-10', 4, 0, @ExpiredUntil, 'EXPIRED'),
    ('WH-01-200', 'Gudang RAW', '5Z5682', 'Material', 'KARTON TOP', 'Pcs', '0882-0005', '2025-02-28', '2025-02-28', 5, 0, @ExpiredUntil, 'EXPIRED'),
    ('WH-01-300', 'Gudang RAW', '5Z5683', 'Material', 'KARTON TOP', 'Pcs', '0882-0006', '2025-02-28', '2025-02-28', 5, 0, @ExpiredUntil, 'EXPIRED');

    ----------------------------------------------------------------
    -- 2?? AMBIL FILTER DARI JSON (OPTIONAL)
    ----------------------------------------------------------------
    DECLARE @TotalRow int
    DECLARE @offset int
    DECLARE @sqlSort varchar(max) = ''

    SET @offset = (@Page - 1) * @Length

    IF ISNULL(@Sort, '') <> ''
	BEGIN
		SET @sqlSort = 'order by ' + @Sort
	END 
	ELSE 
	BEGIN
		SET @sqlSort = 'order by a.ReceiptNo'
	END 

    --IF (@Filters IS NOT NULL AND LEN(@Filters) > 0)
    --BEGIN
    --    SELECT
    --        @ExpiredUntil = JSON_VALUE(@Filters, '$[0].ExpiredUntil');
    --END

    IF @ExpiredUntil is null
	BEGIN
		SET @ExpiredUntil = CAST('2000-01-01' AS DATE)
	END

    IF (SELECT COUNT(*) FROM #Data) <> 0
    BEGIN
        SET @TotalRow = (SELECT COUNT(*) FROM #Data)
    END
    ELSE SET @TotalRow = 0

    

    ----------------------------------------------------------------
    -- 3?? FILTER DATA
    ----------------------------------------------------------------
    DECLARE @sql NVARCHAR(MAX) = N'
        ;WITH FilteredData AS
        (
            SELECT *
            FROM #Data
            WHERE
                (@ExpiredUntil IS NULL OR @ExpiredUntil <= @ExpiredUntil)
            AND 
		    (
			    @Keyword IS NULL 
			    OR WHCode LIKE ''%'' + @Keyword + ''%'' 
			    OR ItemCode LIKE ''%'' + @Keyword + ''%'' 
			    OR PartNo LIKE ''%'' + @Keyword + ''%'' 
			    OR LotNo LIKE ''%'' + @Keyword + ''%''
                OR [Status] LIKE ''%'' + @Keyword + ''%''
		    )
        )

        SELECT @TotalRow as [TotalRows], a.*
        FROM FilteredData a
        ' +
		CASE 
			WHEN ISNULL(@Sort, '') <> '' THEN N' ORDER BY ' + @Sort
			ELSE N' ORDER BY a.ReceiptDate'
		END + N'
        OFFSET @Offset ROWS
		FETCH NEXT @Length ROWS ONLY;
    ';

	print @sql

	EXEC sp_executesql
		@sql,
		N'@Keyword VARCHAR(MAX), @ExpiredUntil DATE, @Offset INT, @Length INT, @TotalRow INT',
		@Keyword = @Keyword,
		@ExpiredUntil = @ExpiredUntil,
		@Offset = @offset,
		@Length = @Length,
		@TotalRow = @TotalRow;

END
GO