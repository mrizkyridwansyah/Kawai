SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [sp_Wms_Receipt_InquiryDetail]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@ReceiptDetailId bigint
as
begin
	declare @sqlSort varchar(max) = ''
	declare @offset int

    -- Hitung offset berdasarkan halaman
    set @offset = (@Page - 1) * @Length

	if isnull(@Sort, '') <> ''
	begin
		set @sqlSort = 'order by ' + @Sort
	end 
	else 
	begin
		set @sqlSort = 'order by a.ReceiptNo'
	end 

	declare @TotalRow int = 
	(
		SELECT COUNT(1) AS TotalRow
		FROM PartReceiptDetailBarcode a
		inner join Item_Master mi on a.ItemCode = mi.Item_Code
		WHERE 1=1
		AND 
		(
			@Keyword IS NULL 
			OR a.BarcodeNo LIKE '%' + @Keyword + '%' 
			OR a.ItemCode LIKE '%' + @Keyword + '%' 
			OR mi.Item_Name LIKE '%' + @Keyword + '%'
		)
		and ReceiptDetailId = @ReceiptDetailId
	)

	DECLARE @sql NVARCHAR(MAX) = N'
		;WITH TargetBarcode AS (
			SELECT DISTINCT
				LotNo,
				BarcodeNo
			FROM PartReceiptDetailBarcode
			WHERE ReceiptDetailId = @ReceiptDetailId
		),
		RankedStock AS (
			SELECT
				sd.BarcodeNo,
				sd.LotNo, 
				sd.ItemCode,
				sd.WarehouseCode,
				sd.AreaCode,
				sd.AddressCode,
				sd.Qty,
				sd.InventoryQty,

				ROW_NUMBER() OVER (
					PARTITION BY sd.BarcodeNo
					ORDER BY
						CASE 
							WHEN sd.Qty > 0 THEN 0      
							ELSE 1
						END,
						ISNULL(sd.LastUpdate, sd.RegisterDate) DESC
				) AS rn
			FROM StockDetail sd
			INNER JOIN TargetBarcode tb ON sd.BarcodeNo = tb.BarcodeNo AND sd.LotNo = tb.LotNo
		)

		SELECT 
			a.ItemCode,
			mi.Item_Name AS ItemName,
			a.LotNo,
			a.BarcodeNo,
			a.SublotNo,
			a.Qty,
			stok.WarehouseCode,
			stok.WarehouseName,
			stok.AreaCode,
			stok.AreaName,
			stok.AddressCode,
			stok.AddressName,
			stok.CurrentQty,
			a.IsVerified, a.VerifiedBy, a.VerifiedDate, @TotalRow TotalRows
		FROM PartReceiptDetailBarcode a
		INNER JOIN Item_Master mi 
			ON a.ItemCode = mi.Item_Code
		LEFT JOIN (
			SELECT  
				rs.BarcodeNo,
				rs.LotNo,
				rs.WarehouseCode,
				mw.WH_Name AS WarehouseName,
				rs.AreaCode,
				ISNULL(ma.AreaName, ''Temporary'') AS AreaName,
				rs.AddressCode,
				ISNULL(mad.AddressName, ''Temporary'') AS AddressName,
				rs.Qty AS CurrentQty
			FROM RankedStock rs
			LEFT JOIN Warehouse_Master mw ON rs.WarehouseCode = mw.WH_Code
			LEFT JOIN MS_Area ma ON rs.AreaCode = ma.AreaCode
			LEFT JOIN MS_Address mad ON rs.AddressCode = mad.AddressCode
			WHERE rs.rn = 1
		) stok 
			ON a.LotNo = stok.LotNo
		   AND a.BarcodeNo = stok.BarcodeNo
		WHERE a.ReceiptDetailId = @ReceiptDetailId
		  AND (
				@Keyword IS NULL
				OR a.BarcodeNo LIKE ''%'' + @Keyword + ''%''
				OR a.ItemCode LIKE ''%'' + @Keyword + ''%''
				OR mi.Item_Name LIKE ''%'' + @Keyword + ''%''
			  )
		
		' +
		CASE 
			WHEN ISNULL(@Sort, '') <> '' THEN N' ORDER BY ' + @Sort
			ELSE N' ORDER BY a.BarcodeNo'
		END + N'
		OFFSET @Offset ROWS
		FETCH NEXT @Length ROWS ONLY;
	';
	print @sql

	EXEC sp_executesql
		@sql,
		N'@Keyword VARCHAR(MAX), @ReceiptDetailId BIGINT, @Offset INT, @Length INT, @TotalRow INT',
		@Keyword = @Keyword,
		@ReceiptDetailId = @ReceiptDetailId,
		@Offset = @offset,
		@Length = @Length,
		@TotalRow = @TotalRow;
	
end
GO
