SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [sp_Wms_ReceiptUnschedule_ListItem]
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
		set @sqlSort = 'order by a.Item_Code'
	end 

	declare @TotalRow int = 
	(
		select count(1)
		From Item_Master a
		inner join Trade_Master b on a.Supplier_Code = b.Trade_Code
		left join ItemSupplierPacking c on a.Item_Code = c.ItemCode and c.SupplierCode = @SupplierCode
		WHERE 1=1
		AND (@Keyword IS NULL OR a.Item_Code LIKE '%' + @Keyword + '%' OR a.Item_Name LIKE '%' + @Keyword + '%')
	)

	declare @sql VARCHAR(MAX) =
		'
		select
			TotalRows = '''+ cast(@TotalRow as varchar) + ''',
			a.Supplier_Code SupplierCode, b.Trade_Name SupplierName, a.Item_Code ItemCode, a.Item_Name ItemName, isnull(c.QtyPacking, a.Number_Box) QtyPacking, LastUpdate = isnull(a.Last_Update, a.Register_Date), 
			d.FullName LastUser, a.Unit_Cls UnitClsCode, e.Description UnitClsName
		From Item_Master a
		inner join Trade_Master b on a.Supplier_Code = b.Trade_Code
		left join ItemSupplierPacking c on a.Item_Code = c.ItemCode and c.SupplierCode = '''+ @SupplierCode + '''
		left join vw_User d on a.Last_User = d.UserID
		left join Unit_Cls e on a.Unit_Cls = e.Unit_Cls
		WHERE 1=1
		AND (''%'+@Keyword+'%'' IS NULL OR a.Item_Code LIKE ''%' + @Keyword + '%'' OR a.Item_Name LIKE ''%' + @Keyword + '%'')
		'+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)
end
GO
