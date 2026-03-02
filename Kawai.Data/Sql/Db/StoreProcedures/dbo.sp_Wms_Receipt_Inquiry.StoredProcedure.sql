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
	@ReceiptId bigint	= null,
	@PeriodFrom date			= null,
	@PeriodUntil date			= null,
	@CompleteStatus varchar(10) = 'ALL'
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
		LEFT JOIN 
		(
			select 
				ReceiptId,
				SUM(case when isnull(IsVerified, 0) = 1 then 1 else 0 end) TotalScan, 
				SUM(case when isnull(IsVerified, 0) = 0 then 1 else 0 end) OutstandingScan 
				from PartReceiptDetailBarcode 
			group by ReceiptId
		) dtlb  on a.Id = dtlb.ReceiptId
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
		and 1 = case when isnull(@ReceiptId, 0) = 0 THEN 1 WHEN isnull(@ReceiptId, 0) = a.Id THEN 1 ELSE 0 END
		and a.ReceiptDate between @PeriodFrom and @PeriodUntil
		and 1 = case when @SupplierCode = 'ALL' THEN 1 WHEN @SupplierCode = SupplierCode THEN 1 ELSE 0 END
		and 1 = case when @FactoryCode = 'ALL' THEN 1 WHEN @FactoryCode = a.CompanyCode THEN 1 ELSE 0 END
		and 1 = case when isnull(@CompleteStatus, 'ALL') = 'ALL' then 1 
				   when @CompleteStatus = 'YES' and isnull(OutstandingScan, 1) = 0 then 1
				   when @CompleteStatus = 'NO' and isnull(OutstandingScan, 1) > 0 then 1
				   else 0 end
	)

	DECLARE @sql NVARCHAR(MAX) = N'
		SELECT
			TotalRows = @TotalRow,
			a.Id,
			dtl.Id ReceiptDetailId,
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
			isnull(xx.QtyScan, 0) QtyScan,
			cc.Description Currency,
			Price = isnull(pod.Price, pm.Price),
			Amount = isnull(pod.Price, pm.Price) * dtl.ReceiptQty
		FROM PartReceiptHeader a
		inner join PartReceiptDetail dtl on a.Id = dtl.ReceiptId
		inner join Company_Profile fak on a.CompanyCode = fak.Company_Code
		inner join Item_Master mi on dtl.ItemCode = mi.Item_Code
		left join 
		(
			select ReceiptDetailId, ReceiptId, sum(Qty) QtyScan 
			From PartReceiptDetailBarcode
			where isnull(IsVerified, 0) = 1
			group by ReceiptDetailId, ReceiptId
		) xx on a.Id = xx.ReceiptId and dtl.Id = xx.ReceiptDetailId
		LEFT JOIN 
		(
			select 
				ReceiptId,
				SUM(case when isnull(IsVerified, 0) = 1 then 1 else 0 end) TotalScan, 
				SUM(case when isnull(IsVerified, 0) = 0 then 1 else 0 end) OutstandingScan 
				from PartReceiptDetailBarcode 
			group by ReceiptId
		) dtlb  on a.Id = dtlb.ReceiptId
		left join PurchaseOrder_Detail pod on dtl.PONumber = pod.PO_No and dtl.ItemCode = pod.Item_Code
		left join Unit_Cls uc on dtl.UnitCls = uc.Unit_Cls
		LEFT JOIN trade_master b ON a.SupplierCode = b.Trade_Code
		left join Price_Master pm on dtl.ItemCode = pm.Item_Code and a.SupplierCode = pm.Trade_Code and Price_Cls = ''01'' AND a.ReceiptDate BETWEEN 
			(select dbo.ConvertToDateTimeFromFuckingString(Start_Date)) and 
			(select dbo.ConvertToDateTimeFromFuckingString(End_Date))
		left join Curr_Cls cc on cc.Curr_Cls = isnull(pod.Currency_Code, pm.Currency_Code)
		WHERE
			(@Keyword IS NULL OR
				a.DNNumber LIKE ''%'' + @Keyword + ''%'' OR
				dtl.PONumber LIKE ''%'' + @Keyword + ''%'' OR
				dtl.ItemCode LIKE ''%'' + @Keyword + ''%'' OR
				mi.Item_Name LIKE ''%'' + @Keyword + ''%'')
			and 1 = case when isnull(@ReceiptId, 0) = 0 THEN 1 WHEN isnull(@ReceiptId, 0) = a.Id THEN 1 ELSE 0 END
			AND a.ReceiptDate BETWEEN @PeriodFrom AND @PeriodUntil
			and 1 = case when @SupplierCode = ''ALL'' THEN 1 WHEN @SupplierCode = SupplierCode THEN 1 ELSE 0 END
			and 1 = case when @FactoryCode = ''ALL'' THEN 1 WHEN @FactoryCode = CompanyCode THEN 1 ELSE 0 END
			and 1 = case when isnull(@CompleteStatus, ''ALL'') = ''ALL'' then 1 
				   when @CompleteStatus = ''YES'' and isnull(OutstandingScan, 1) = 0 then 1
				   when @CompleteStatus = ''NO'' and isnull(OutstandingScan, 1) > 0 then 1
				   else 0 end
			
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
		N'@Keyword VARCHAR(MAX), @FactoryCode VARCHAR(25), @SupplierCode VARCHAR(25), @ReceiptId BIGINT, @PeriodFrom DATE, @PeriodUntil DATE, @CompleteStatus VARCHAR(10), @Offset INT, @Length INT, @TotalRow INT',
		@Keyword = @Keyword,
		@FactoryCode = @FactoryCode,
		@SupplierCode = @SupplierCode,
		@ReceiptId = @ReceiptId,
		@PeriodFrom = @PeriodFrom,
		@PeriodUntil = @PeriodUntil,
		@CompleteStatus = @CompleteStatus,
		@Offset = @offset,
		@Length = @Length,
		@TotalRow = @TotalRow;
	
end
GO
