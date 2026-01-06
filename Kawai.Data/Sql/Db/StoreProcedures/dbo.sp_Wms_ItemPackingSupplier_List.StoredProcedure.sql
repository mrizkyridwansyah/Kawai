SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_ItemPackingSupplier_List]
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
		set @sqlSort = 'order by a.ItemCode'
	end 


	declare @TotalRow int = 
	(
		select count(1)
		From ItemSupplierPacking a
		inner join Trade_Master b on a.SupplierCode = b.Trade_Code
		inner join Item_Master c on a.ItemCode = c.Item_Code
		WHERE 1=1
		AND (@Keyword IS NULL OR a.ItemCode LIKE '%' + @Keyword + '%' OR c.Item_Name LIKE '%' + @Keyword + '%')
		and 1 = case when @SupplierCode = 'ALL' or @SupplierCode = a.SupplierCode then 1 else 0 end
	)

	declare @sql VARCHAR(MAX) =
		'
		select
			TotalRows = '''+ cast(@TotalRow as varchar) + ''',
			SupplierCode, b.Trade_Name SupplierName, ItemCode, c.Item_Name ItemName, QtyPacking, LastUpdate = isnull(a.LastUpdate, a.RegisterDate), 
			d.FullName LastUser, a.UnitCls UnitClsCode, e.Description UnitClsName
		From ItemSupplierPacking a
		inner join Trade_Master b on a.SupplierCode = b.Trade_Code
		inner join Item_Master c on a.ItemCode = c.Item_Code
		inner join vw_User d on isnull(a.LastUser, a.RegisterUser) = d.UserID
		left join Unit_Cls e on c.Unit_Cls = e.Unit_Cls
		WHERE 1=1
		AND (''%'+@Keyword+'%'' IS NULL OR a.ItemCode LIKE ''%' + @Keyword + '%'' OR c.Item_Name LIKE ''%' + @Keyword + '%'')
		and 1 = case when '''+ @SupplierCode +''' = ''ALL'' or '''+ @SupplierCode +''' = a.SupplierCode then 1 else 0 end
		'+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)
end
GO
