SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [sp_Wms_PO_ListDetail]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@PONumber varchar(25) = '',
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
		FROM PurchaseOrder_Detail a
		inner join PurchaseOrder_Master b on a.PO_No = b.PO_No
		left join trade_master c on b.Supplier_Code = c.Trade_Code
		left join Item_Master d on a.Item_Code = d.Item_Code
		WHERE 1=1
		AND (@Keyword IS NULL OR a.PO_No LIKE '%' + @Keyword + '%' OR b.Supplier_Code LIKE '%' + @Keyword + '%' OR c.Trade_Name LIKE '%' + @Keyword + '%'  OR d.Item_Name LIKE '%' + @Keyword + '%')
		and (@SupplierCode = '' or b.Supplier_Code = @SupplierCode)
		and (@PONumber = '' or a.PO_No = @PONumber)
	)

	declare @sql VARCHAR(MAX) =
		'
		select
			TotalRows = '''+ cast(@TotalRow as varchar) + ''',
			a.PO_No PONumber, b.PO_Date PODate, b.Supplier_Code SupplierCode, c.Trade_Name SupplierName, 
			a.Item_Code ItemCode, d.Item_Name ItemName, a.Unit_Cls UnitClsCode, e.Description UnitClsName, a.Qty, 5 TotalPacking, 
			isnull(f.QtyPacking, 0) QtyPacking
		FROM PurchaseOrder_Detail a
		inner join PurchaseOrder_Master b on a.PO_No = b.PO_No
		left join trade_master c on b.Supplier_Code = c.Trade_Code
		left join Item_Master d on a.Item_Code = d.Item_Code
		left join Unit_Cls e on a.Unit_Cls = e.Unit_Cls
		left join ItemSupplierPacking f on a.Item_Code = f.ItemCode and b.Supplier_Code = f.SupplierCode
		WHERE 1=1
		AND (''%'+@Keyword+'%'' IS NULL OR a.PO_No LIKE ''%' + @Keyword + '%'' OR b.Supplier_Code LIKE ''%' + @Keyword + '%'' OR c.Trade_Name LIKE ''%' + @Keyword + '%'' OR d.Item_Name LIKE ''%' + @Keyword + '%'')
		and (''' + @SupplierCode + ''' = '''' or b.Supplier_Code = ''' + @SupplierCode + ''')
		and (''' + @PONumber + ''' = '''' or a.PO_No = ''' + @PONumber + ''')
		'+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)
	
end
GO
