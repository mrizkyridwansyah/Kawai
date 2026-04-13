SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [sp_Wms_PartMaterialRequest_GetListStock]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@ItemCode varchar(25)
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
		set @sqlSort = 'order by BarcodeNo'
	end 

	declare @TotalRows int = 
	(
		select count (1)
		from StockDetail
		where 1=1
		and (BarcodeNo like '%'+@Keyword+'%') 
		and ItemCode = @ItemCode 
		and Qty > 0
		and Picking_No is null
	)

	declare @sql varchar(max) = 
	'
		select 
			sd.WarehouseCode, mw.WarehouseName,
			sd.AreaCode, isnull(ma.AreaName, ''Temporary'') AreaName,
			sd.AddressCode, isnull(mad.AddressName, ''Temporary'') AddressName,
			sd.ItemCode, mi.Item_Name ItemName, sd.LotNo, sd.BarcodeNo, sd.SublotNo, sd.Qty CurrentQty, isnull(sd.LastUpdate, sd.RegisterDate) LastUpdate, us.FullName LastUser, '''+cast(@TotalRows as varchar)+''' TotalRows
		from StockDetail sd
		left join vw_WarehouseLine mw on sd.WarehouseCode = mw.WarehouseCode
		left join MS_Area ma on sd.AreaCode = ma.AreaCode
		left join vw_Address mad on sd.AddressCode = mad.AddressCode
		left join Item_Master mi on sd.ItemCode = mi.Item_Code
		left join vw_User us on isnull(sd.LastUser, sd.RegisterUser) = us.UserID
		where 1=1
		and (BarcodeNo like ''%'+@Keyword+'%'') 
		and sd.ItemCode = '''+ @ItemCode + ''' and sd.Qty > 0
		and sd.Picking_No is null
		'+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)
end
GO
