SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_DeliveryNote_List]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@IsComplete bit = null
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
		set @sqlSort = 'order by a.SupplierCode'
	end 

	IF OBJECT_ID('tempdb..#tblTemp') IS NOT NULL
		DROP TABLE #tblTemp;

	create table #tblTemp 
	(
		DNNumber varchar(100),
		SupplierCode varchar(15),
		DNDate date,
		BCNumber varchar(10),
		BCType varchar(25),
		BCDate date,
		VehicleNo varchar(15),
		IsComplete bit
	)

	insert into #tblTemp
	select '123123', 'S0331', cast(dateadd(day, -7, getdate()) as date), '123', 'BC 2.3', cast(dateadd(day, -7, getdate()) as date), 'B 123 TES', CAST(0 AS BIT)
	UNION all
	select '456456', 'S0012', cast(dateadd(day, -5, getdate()) as date), '456', 'BC 2.5', cast(dateadd(day, -5, getdate()) as date), 'B 456 TES', CAST(0 AS BIT)
	UNION all
	select '789789', 'S0031', cast(dateadd(day, -1, getdate()) as date), '789', 'BC 2.5', cast(dateadd(day, -1, getdate()) as date), 'B 789 TES', CAST(0 AS BIT)
	UNION all
	select '234234', 'S0331', cast(dateadd(day, -7, getdate()) as date), '123', 'BC 2.3', cast(dateadd(day, -7, getdate()) as date), 'B 123 TES', CAST(0 AS BIT)
	UNION all
	select '345345', 'S0012', cast(dateadd(day, -5, getdate()) as date), '456', 'BC 2.5', cast(dateadd(day, -5, getdate()) as date), 'B 456 TES', CAST(0 AS BIT)
	UNION all
	select '567567', 'S0031', cast(dateadd(day, -1, getdate()) as date), '789', 'BC 2.5', cast(dateadd(day, -1, getdate()) as date), 'B 789 TES', CAST(0 AS BIT)
	UNION all
	select '678678', 'S0331', cast(dateadd(day, -7, getdate()) as date), '123', 'BC 2.3', cast(dateadd(day, -7, getdate()) as date), 'B 123 TES', CAST(0 AS BIT)
	UNION all
	select '321321', 'S0012', cast(dateadd(day, -5, getdate()) as date), '456', 'BC 2.5', cast(dateadd(day, -5, getdate()) as date), 'B 456 TES', CAST(0 AS BIT)
	UNION all
	select '432432', 'S0031', cast(dateadd(day, -1, getdate()) as date), '789', 'BC 2.5', cast(dateadd(day, -1, getdate()) as date), 'B 789 TES', CAST(0 AS BIT)
	UNION all
	select '543543', 'S0331', cast(dateadd(day, -7, getdate()) as date), '123', 'BC 2.3', cast(dateadd(day, -7, getdate()) as date), 'B 123 TES', CAST(0 AS BIT)
	UNION all
	select '654654', 'S0012', cast(dateadd(day, -5, getdate()) as date), '456', 'BC 2.5', cast(dateadd(day, -5, getdate()) as date), 'B 456 TES', CAST(0 AS BIT)
	UNION all
	select '765765', 'S0031', cast(dateadd(day, -1, getdate()) as date), '789', 'BC 2.5', cast(dateadd(day, -1, getdate()) as date), 'B 789 TES', CAST(0 AS BIT)
	
	declare @TotalRow int = 
	(
		SELECT COUNT(1) AS TotalRow
		FROM #tblTemp 
		WHERE 1=1
		AND (@Keyword IS NULL OR DNNumber LIKE '%' + @Keyword + '%' OR BCNumber LIKE '%' + @Keyword + '%')
		and (@IsComplete IS NULL or IsComplete = @IsComplete)
	)

	declare @sql VARCHAR(MAX) =
		'select
			TotalRows				= '''+ cast(@TotalRow as varchar) + ''',
			a.DNNumber, a.SupplierCode, b.Trade_Name SupplierName, a.DNDate, a.BCNumber, a.BCType, a.BCDate, a.VehicleNo, a.IsComplete
		FROM #tblTemp a
		left join trade_master b on a.SupplierCode = b.Trade_Code
		WHERE 1=1
		and (DNNumber like ''%'+@Keyword+'%'' or BCNumber like ''%'+@Keyword+'%'')
		AND (' + ISNULL(CAST(@IsComplete AS VARCHAR), 'NULL') + ' IS NULL OR a.IsComplete = ' + ISNULL(CAST(@IsComplete AS VARCHAR), 'NULL') + ')
		'+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)
	
end
GO
