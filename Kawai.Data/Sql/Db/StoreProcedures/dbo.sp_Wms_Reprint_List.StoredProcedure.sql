



CREATE PROCEDURE [dbo].[sp_Wms_Reprint_List] 
--declare
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 100,
	@Sort varchar(max) = '',
	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@WarehouseCode varchar(max)='WH-001',
	@AreaCode varchar(max)='ALL',
	@AddressCode varchar(max)='ALL'
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
		set @sqlSort = 'order by ml.BarcodeNo'
	end 

	declare @sql varchar(max) = 
	'
		Select BarcodeNo	
		,ItemCode	
		,ItemName	
		,Warehouse	
		,Area	
		,[Address]
		,LotNo	
		,SublotNo	
		,PrintDate
		,PrintUser	
		,[Source]	
		,WarehouseCode	
		,AreaCode	
		,AddressCode , Qty ,
		Count(1) over () TotalRows   from 
		(select A.BarcodeNo , A.ItemCode, C.Item_Name as ItemName ,  D.WH_Name as Warehouse ,  ISNULL(E.AreaName,''Temporary'') as Area ,
		ISNULL(F.AddressName,''Temporary'') as [Address] ,
		A.LotNo , A.SublotNo , PrintDate , PrintUser , ''Receipt'' [Source], B.WarehouseCode , B.AreaCode , B.AddressCode , B.Qty
		  from PartReceiptDetailBarcode A Inner JOIN StockDetail B ON A.BarcodeNo = B.BarcodeNo
		Left JOIN Item_Master C ON C.Item_Code = A.ItemCode
		Left JOIN WareHouse_Master D ON D.WH_Code = B.WarehouseCode
		LEFT JOIN MS_Area E ON E.AreaCode = B.AreaCode and E.WarehouseCode = B.WarehouseCode
		LEFT JOIN MS_Address F ON F.AddressCode = B.AddressCode and F.WarehouseCode = B.WarehouseCode and F.AreaCode = B.AreaCode
		where B.Qty > 0
		UNION ALL
		select A.BarcodeNo , A.Item_Code ItemCode, C.Item_Name as ItemName ,  D.WH_Name as Warehouse ,  ISNULL(E.AreaName,''Temporary'') as Area ,
		ISNULL(F.AddressName,''Temporary'') as [Address] ,
		A.Lot_No LotNo , A.SublotNo , A.Last_update PrintDate , A.Last_User PrintUser , ''BarcodeSplit'' [Source], B.WarehouseCode , B.AreaCode , B.AddressCode, B.Qty
		 from Barcode_Split A Inner JOIN StockDetail B ON A.BarcodeNo = B.BarcodeNo
		Left JOIN Item_Master C ON C.Item_Code = A.Item_Code
		Left JOIN WareHouse_Master D ON D.WH_Code = B.WarehouseCode
		LEFT JOIN MS_Area E ON E.AreaCode = B.AreaCode and E.WarehouseCode = B.WarehouseCode
		LEFT JOIN MS_Address F ON F.AddressCode = B.AddressCode and F.WarehouseCode = B.WarehouseCode and F.AreaCode = B.AreaCode
		where B.Qty > 0
		) ml
		where 1=1
		and (ml.BarcodeNo like ''%'+@Keyword+'%'' or ml.ItemCode like ''%'+@Keyword+'%'' or ml.ItemName like ''%'+@Keyword+'%'')
	    and 1 = case when '''+ @WarehouseCode +''' = ''ALL'' or '''+ @WarehouseCode +''' = ml.WarehouseCode then 1 else 0 end
		and 1 = case when '''+ @AreaCode +''' = ''ALL'' or '''+ @AreaCode +''' = ml.AreaCode then 1 else 0 end
		and 1 = case when '''+ @AddressCode +''' = ''ALL'' or '''+ @AddressCode +''' = ml.AddressCode then 1 else 0 end
		'+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)

end
