
create   procedure [dbo].[sp_Wms_StockInquiry_InquiryByCategory]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',

	-- PARAMETER FILTER
	@Category varchar(25) = '02',
	@WarehouseCode varchar(25) = 'ALL',
	@AreaCode varchar(25) = 'ALL',
	@ItemCode varchar(25) = 'ALL',
	@LotNo varchar(100)	 = 'ALL'
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
		set @sqlSort = 'order by mi.Item_Name'
	end 

	declare @TotalRows int = 
	(
		select count(1) TotalRow
		from 
		(
			select WarehouseCode
			From StockDetail a inner join 
			(
				select Item_Code From Item_Master where ClasificationPart_Cls = @Category
			) mi on a.ItemCode = mi.Item_Code
			where 1=1 
			and Qty > 0
			and (@WarehouseCode = 'ALL' or @WarehouseCode = WarehouseCode)
			and (@AreaCode = 'ALL' or @AreaCode = AreaCode)
			and (@ItemCode = 'ALL' or @ItemCode = ItemCode)
			and (@LotNo = 'ALL' or @LotNo = LotNo)
			group by WarehouseCode
		) res
	)

	declare @sql varchar(max) = 
	'
		select 
			sd.*,
			ReceiptQty = sm.TMReceipt,
			SupplyQty = sm.TMSupply,
			PreMonthQty = sm.TMPreMonth, TotalRows
		from
		(
			select 
				sd.RefNo,
				sd.WarehouseCode WarehouseCode, xx.WarehouseName,
				sd.AreaCode, xx.AreaName,
				sd.AddressCode, xx.AddressName,
				sd.ItemCode ItemCode, xx.ItemName,
				sd.LotNo LotNo,  xx.TotalRows,
				CurrentQty = ISNULL(SUM(sd.Qty),0)
			From StockDetail sd
			inner join 
			(
				select RefNo, WarehouseCode, AreaCode, AddressCode, ItemCode, WarehouseName, AreaName, AddressName, ItemName, LotNo, TotalRows
				from 
				(
					select 
						sdt.RefNo, sdt.WarehouseCode, sdt.AreaCode, sdt.AddressCode, sdt.ItemCode, 
						mw.WarehouseName, isnull(ml.AreaName, ''Temporary'') AreaName, isnull(mx.AddressName, ''Temporary'') AddressName, mi.Item_Name ItemName, sdt.LotNo, mi.TotalRows
					From StockDetail sdt
					inner join 
					(
						select sdx.ItemCode, mi.Item_Name, count(1) over () TotalRows
						From StockDetail sdx 
						INNER join 
						(
							select * from Item_Master where ClasificationPart_Cls = '''+ @Category + '''
						) mi on sdx.ItemCode = mi.Item_Code
						where 1=1
						and sdx.Qty > 0
						and ('''+ @WarehouseCode + ''' = ''ALL'' or '''+ @WarehouseCode + ''' = sdx.WarehouseCode)
						and ('''+ @AreaCode + ''' = ''ALL'' or '''+ @AreaCode + ''' = sdx.AreaCode)
						and ('''+ @ItemCode + ''' = ''ALL'' or '''+ @ItemCode + ''' = sdx.ItemCode)
						and ('''+ @LotNo + ''' = ''ALL'' or '''+ @LotNo + ''' = sdx.LotNo)
						group by sdx.ItemCode, mi.Item_Name
						'+ @sqlSort +'
						OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
						FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
					) mi on sdt.ItemCode = mi.ItemCode
					left join 
					(
						SELECT WH_Code WarehouseCode, WH_Name WarehouseName fROM Warehouse_Master
						UNION ALL
						SELECT Line_Code, Line_Name FROM Manufacture_Line
					) mw on sdt.WarehouseCode = mw.WarehouseCode
					left join 
					(
						SELECT AreaCode, AreaName fROM MS_Area
						UNION ALL
						SELECT Line_Code, Line_Name FROM Manufacture_Line
					) ml on sdt.AreaCode = ml.AreaCode
					left join 
					(
						SELECT AddressCode, AddressName fROM MS_Address
						UNION ALL
						SELECT Line_Code, Line_Name FROM Manufacture_Line
					) mx on sdt.AddressCode = mx.AddressCode
					where 1=1 
					and ('''+ @WarehouseCode + ''' = ''ALL'' or '''+ @WarehouseCode + ''' = sdt.WarehouseCode)
					and ('''+ @AreaCode + ''' = ''ALL'' or '''+ @AreaCode + ''' = sdt.AreaCode)
					and ('''+ @ItemCode + ''' = ''ALL'' or '''+ @ItemCode + ''' = sdt.ItemCode)
					and ('''+ @LotNo + ''' = ''ALL'' or '''+ @LotNo + ''' = sdt.LotNo)
					group by sdt.RefNo, sdt.WarehouseCode, sdt.AreaCode, sdt.AddressCode, sdt.ItemCode, mw.WarehouseName, isnull(ml.AreaName, ''Temporary''), isnull(mx.AddressName, ''Temporary''), mi.Item_Name, sdt.LotNo, mi.TotalRows
				) res
			) xx 
			on sd.RefNo = xx.RefNo and sd.WarehouseCode = xx.WarehouseCode and sd.AreaCode = xx.AreaCode and sd.AddressCode = xx.AddressCode 
			and sd.ItemCode = xx.ItemCode and sd.LotNo = xx.LotNo
			where sd.qty > 0
			group by 
				sd.RefNo,
				sd.WarehouseCode, xx.WarehouseName,
				sd.AreaCode, xx.AreaName,
				sd.AddressCode, xx.AddressName,
				sd.ItemCode, xx.ItemName,
				sd.LotNo, xx.TotalRows
		) sd
		outer apply
		(
			select * From StockHeader sm2 
			where sm2.RefNo = sd.RefNo and sm2.WarehouseCode = sd.WarehouseCode AND sm2.AreaCode = sd.AreaCode 
			and sm2.ItemCode = sd.ItemCode and sm2.LotNo = sd.LotNo
		) sm
		order by sd.ItemName, sd.WarehouseName, sd.AreaName, sd.AddressName, sd.LotNo

	'

	print @sql

	execute (@sql)
end
