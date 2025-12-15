SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Receipt_Inquiry]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@FactoryCode varchar(25)	= null,
	@SupplierCode varchar(25)	= null,
	@PeriodFrom date			= null,
	@PeriodUntil date			= null
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

	if @PeriodFrom is null
	begin
		set @PeriodFrom = cast('2000-01-01' as date)
	end

	if @PeriodUntil is null
	begin
		set @PeriodUntil = cast(getdate() as date)
	end

	if @PeriodFrom > @PeriodUntil
	begin
		DECLARE @temp DATE;
		SET @temp = @PeriodFrom;
		SET @PeriodFrom = @PeriodUntil;
		SET @PeriodUntil = @temp;
	end

	declare @TotalRow int = 
	(
		SELECT COUNT(1) AS TotalRow
		FROM PartReceiptHeader a
		inner join PartReceiptDetail dtl on a.Id = dtl.ReceiptId
		inner join Item_Master mi on dtl.ItemCode = mi.Item_Code
		left join trade_master b on a.SupplierCode = b.Trade_Code
		WHERE 1=1
		AND 
		(
			@Keyword IS NULL 
			OR DNNumber LIKE '%' + @Keyword + '%' 
			OR PONumber LIKE '%' + @Keyword + '%' 
			OR ItemCode LIKE '%' + @Keyword + '%' 
			OR mi.Item_Name LIKE '%' + @Keyword + '%' 
		)
		and a.ReceiptDate between @PeriodFrom and @PeriodUntil
		and 1 = case when @SupplierCode = 'ALL' THEN 1 WHEN @SupplierCode = SupplierCode THEN 1 ELSE 0 END
		and 1 = case when @FactoryCode = 'ALL' THEN 1 WHEN @FactoryCode = a.CompanyCode THEN 1 ELSE 0 END
	)

	DECLARE @sql NVARCHAR(MAX) = N'
		SELECT
			TotalRows = @TotalRow,
			a.Id,
			a.ReceiptNo, 
			a.ReceiptDate,
			a.CompanyCode FactoryCode,
			fak.Company_Name FactoryName,
			a.SupplierCode,
			b.Trade_Name AS SupplierName,
			dtl.ItemCode,
			mi.Item_Name ItemName,
			a.DNNumber,
			a.DNDate,
			dtl.PONumber,
			a.BCNumber,
			a.BCType,
			a.BCDate,
			dtl.UnitCls,
			uc.Description UnitClsDescription,
			dtl.ReceiptQty Qty,
			cc.Description Currency,
			pod.Price,
			Amount = pod.Price * dtl.ReceiptQty
		FROM PartReceiptHeader a
		inner join PartReceiptDetail dtl on a.Id = dtl.ReceiptId
		inner join Company_Profile fak on a.CompanyCode = fak.Company_Code
		inner join Item_Master mi on dtl.ItemCode = mi.Item_Code
		left join PurchaseOrder_Detail pod on dtl.PONumber = pod.PO_No and dtl.ItemCode = pod.Item_Code
		left join Unit_Cls uc on dtl.UnitCls = uc.Unit_Cls
		LEFT JOIN trade_master b ON a.SupplierCode = b.Trade_Code
		left join Curr_Cls cc on cc.Curr_Cls = pod.Currency_Code
		WHERE
			(@Keyword IS NULL OR
				a.DNNumber LIKE ''%'' + @Keyword + ''%'' OR
				dtl.PONumber LIKE ''%'' + @Keyword + ''%'' OR
				dtl.ItemCode LIKE ''%'' + @Keyword + ''%'' OR
				mi.Item_Name LIKE ''%'' + @Keyword + ''%'')
			AND a.ReceiptDate BETWEEN @PeriodFrom AND @PeriodUntil
			and 1 = case when @SupplierCode = ''ALL'' THEN 1 WHEN @SupplierCode = SupplierCode THEN 1 ELSE 0 END
			and 1 = case when @FactoryCode = ''ALL'' THEN 1 WHEN @FactoryCode = CompanyCode THEN 1 ELSE 0 END
			
		' +
		CASE 
			WHEN ISNULL(@Sort, '') <> '' THEN N' ORDER BY ' + @Sort
			ELSE N' ORDER BY a.ReceiptNo'
		END + N'
		OFFSET @Offset ROWS
		FETCH NEXT @Length ROWS ONLY;
	';
	print @sql

	EXEC sp_executesql
		@sql,
		N'@Keyword VARCHAR(MAX), @FactoryCode VARCHAR(25), @SupplierCode VARCHAR(25), @PeriodFrom DATE, @PeriodUntil DATE, @Offset INT, @Length INT, @TotalRow INT',
		@Keyword = @Keyword,
		@FactoryCode = @FactoryCode,
		@SupplierCode = @SupplierCode,
		@PeriodFrom = @PeriodFrom,
		@PeriodUntil = @PeriodUntil,
		@Offset = @offset,
		@Length = @Length,
		@TotalRow = @TotalRow;
	
end
GO
