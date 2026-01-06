SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_PO_List]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@SupplierCode varchar(25) = ''
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
		set @sqlSort = 'order by a.PO_No'
	end 


	declare @TotalRow int = 
	(
		select count(1)
		FROM PurchaseOrder_Master a
		left join trade_master b on a.Supplier_Code = b.Trade_Code
		WHERE 1=1
		AND (@Keyword IS NULL OR PO_No LIKE '%' + @Keyword + '%' OR Supplier_Code LIKE '%' + @Keyword + '%' OR b.Trade_Name LIKE '%' + @Keyword + '%')
		and (@SupplierCode = '' or a.Supplier_Code = @SupplierCode)
	)

	declare @sql VARCHAR(MAX) =
		'
		select
			TotalRows = '''+ cast(@TotalRow as varchar) + ''',
			a.PO_No PONumber, a.Supplier_Code SupplierCode, b.Trade_Name SupplierName, a.PO_Date PODate, 
			a.WHTo WarehouseCode, c.WH_Name WarehouseName
		FROM PurchaseOrder_Master a
		left join trade_master b on a.Supplier_Code = b.Trade_Code
		left join WareHouse_Master c on a.WHTo= c.WH_Code
		WHERE 1=1
		AND (''%'+@Keyword+'%'' IS NULL OR PO_No LIKE ''%' + @Keyword + '%'' OR Supplier_Code LIKE ''%' + @Keyword + '%'' OR b.Trade_Name LIKE ''%' + @Keyword + '%'')
		and (''' + @SupplierCode + ''' = '''' or a.Supplier_Code = ''' + @SupplierCode + ''')
		'+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)
	
end
GO
