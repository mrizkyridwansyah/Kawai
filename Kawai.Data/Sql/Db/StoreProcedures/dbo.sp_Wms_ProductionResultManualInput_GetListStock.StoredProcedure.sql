
CREATE   procedure [dbo].[sp_Wms_ProductionResultManualInput_GetListStock]
 --Declare
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@ItemCode varchar(25)='956139',
	@ProductionId varchar(25)=1020401
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
		select  count(1)
		From PartMaterialRequestItemDetailScan pmrids
		inner join PartMaterialRequestItemDetail pmrid on pmrids.IDSeq = pmrid.IDSeq
		inner join PartMaterialRequestDetail pmrd on pmrid.RequestDetailID = pmrd.RequestDetailID
		inner join PartMaterialRequestHeader pmrh on pmrh.RequestID = pmrd.RequestID
		Left JOIN Item_Master im on im.Item_Code = pmrids.ItemCode
		where 1=1
		and (BarcodeNo like '%'+@Keyword+'%') 
		and pmrids.ItemCode = @ItemCode 
		and ProductionID = @ProductionID
		and Qty > 0
		 
	)

	declare @sql varchar(max) = 
	'
		select 
			BarcodeNo, Qty as CurrentQty, pmrids.ItemCode  , im.Item_Name as ItemName, '''+cast(@TotalRows as varchar)+''' TotalRows
		From PartMaterialRequestItemDetailScan pmrids
		inner join PartMaterialRequestItemDetail pmrid on pmrids.IDSeq = pmrid.IDSeq
		inner join PartMaterialRequestDetail pmrd on pmrid.RequestDetailID = pmrd.RequestDetailID
		inner join PartMaterialRequestHeader pmrh on pmrh.RequestID = pmrd.RequestID
		Left JOIN Item_Master im on im.Item_Code = pmrids.ItemCode
		where 1=1
		and (BarcodeNo like ''%'+@Keyword+'%'') 
		and pmrids.ItemCode = '''+ @ItemCode + ''' and  Qty > 0
		and ProductionID = '''+ @ProductionID + '''
		'+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)
end
