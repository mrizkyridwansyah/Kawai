SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [sp_Wms_NGClaimMaterial_List]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
 	@SupplierCode varchar(25) = null,
	@PeriodFrom date = null,
	@PeriodUntil date = null
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
		set @sqlSort = 'order by a.ClaimNo'
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
		FROM MaterialNGClaimHeader a
		left join trade_master b on a.SupplierCode = b.Trade_Code
		WHERE 1=1
		AND 
		(
			@Keyword IS NULL 
			OR DNNumber LIKE '%' + @Keyword + '%' 
			OR BCNumber LIKE '%' + @Keyword + '%' 
			OR ClaimNo LIKE '%' + @Keyword + '%'
			OR b.Trade_Name LIKE '%' + @Keyword + '%'
		)
		and ClaimDate between @PeriodFrom and @PeriodUntil
		and 1 = case when @SupplierCode = 'ALL' THEN 1 WHEN @SupplierCode = SupplierCode THEN 1 ELSE 0 END
		 
	)

	DECLARE @sql NVARCHAR(MAX) = N'
		SELECT
			TotalRows = @TotalRow,
			a.ClaimId,
			a.ClaimNo, 
			a.ClaimDate,
			a.DNNumber,
			a.SupplierCode,
			b.Trade_Name AS SupplierName,
			a.DNDate,
			a.BCNumber,
			a.BCType,
			a.BCDate,
			a.VehicleNo,
			a.Transport,
			 a.LastUpdate,
			c.FullName LastUser
		FROM MaterialNGClaimHeader a
		LEFT JOIN trade_master b ON a.SupplierCode = b.Trade_Code
		left join ss_usersetup c on isnull(a.LastUser, a.RegisterUser) = c.UserID
		WHERE
			(@Keyword IS NULL OR
				a.DNNumber LIKE ''%'' + @Keyword + ''%'' OR
				a.BCNumber LIKE ''%'' + @Keyword + ''%'' OR
				a.ClaimNo LIKE ''%'' + @Keyword + ''%'' OR
				b.Trade_Name LIKE ''%'' + @Keyword + ''%'')
			AND a.ClaimDate BETWEEN @PeriodFrom AND @PeriodUntil
			and 1 = case when @SupplierCode = ''ALL'' THEN 1 WHEN @SupplierCode = SupplierCode THEN 1 ELSE 0 END
		 	
		' +
		CASE 
			WHEN ISNULL(@Sort, '') <> '' THEN N' ORDER BY ' + @Sort
			ELSE N' ORDER BY a.ClaimNo'
		END + N'
		OFFSET @Offset ROWS
		FETCH NEXT @Length ROWS ONLY;
	';
	print @sql

	EXEC sp_executesql
		@sql,
		N'@Keyword VARCHAR(MAX),  @SupplierCode VARCHAR(25), @PeriodFrom DATE, @PeriodUntil DATE, @Offset INT, @Length INT, @TotalRow INT',
		@Keyword = @Keyword,
	 	@SupplierCode = @SupplierCode,
		@PeriodFrom = @PeriodFrom,
		@PeriodUntil = @PeriodUntil,
		@Offset = @offset,
		@Length = @Length,
		@TotalRow = @TotalRow;
	
end
GO
