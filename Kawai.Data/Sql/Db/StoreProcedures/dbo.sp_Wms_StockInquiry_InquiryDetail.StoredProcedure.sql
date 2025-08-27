SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_StockInquiry_InquiryDetail]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@WarehouseCode varchar(25),
	@AreaCode varchar(25),
	@AddressCode varchar(25),
	@ItemCode varchar(25),
	@LotNo varchar(100)
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
		set @sqlSort = 'order by SublotNo'
	end 

	declare @TotalRows int = 
	(
		select count (1)
		from StockDetail
		where 1=1
		and (BarcodeNo like '%'+@Keyword+'%') 
		and WarehouseCode = @WarehouseCode and AreaCode = @AreaCode and AddressCode = @AddressCode
		and ItemCode = @ItemCode and LotNo = @LotNo and Qty > 0
	)

	declare @sql varchar(max) = 
	'
		select 
			sd.WarehouseCode, mw.WH_Name WarehouseName,
			sd.AreaCode, isnull(ma.AreaName, ''Temporary'') AreaName,
			sd.AddressCode, isnull(mad.AddressName, ''Temporary'') AddressName,
			sd.ItemCode, mi.Item_Name ItemName, sd.LotNo, sd.BarcodeNo, sd.SublotNo, sd.Qty CurrentQty, isnull(sd.LastUpdate, sd.RegisterDate) LastUpdate, us.FullName LastUser, '''+cast(@TotalRows as varchar)+''' TotalRows
		from StockDetail sd
		left join Warehouse_Master mw on sd.WarehouseCode = mw.WH_Code
		left join MS_Area ma on sd.AreaCode = ma.AreaCode
		left join MS_Address mad on sd.AddressCode = mad.AddressCode
		left join Item_Master mi on sd.ItemCode = mi.Item_Code
		left join vw_User us on sd.LastUser = us.UserID
		where 1=1
		and (BarcodeNo like ''%'+@Keyword+'%'') 
		and sd.WarehouseCode = '''+ @WarehouseCode + ''' and sd.AreaCode = '''+ @AreaCode + ''' and sd.AddressCode = '''+ @AddressCode + '''
		and sd.ItemCode = '''+ @ItemCode + ''' and sd.LotNo = '''+ @LotNo + ''' and sd.Qty > 0
		'+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)
end
GO
