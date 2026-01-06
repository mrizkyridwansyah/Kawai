SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




create   procedure [sp_Wms_Area_List] 
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',
	@GetTotalRow bit = '0',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@WarehouseCode varchar(max)
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
		set @sqlSort = 'order by ml.AreaCode'
	end 

	declare @TotalRows int = 
	(
		select count (1)
		from MS_Area ml
		left join WareHouse_Master mw on mw.WH_Code = ml.WarehouseCode
		left join SS_UserSetup us on ml.RegisterBy = us.UserID
		left join SS_UserSetup us2 on ml.UpdateBy = us2.UserID
		where 1=1
		and (ml.AreaCode like '%'+@Keyword+'%' or ml.AreaName like '%'+@Keyword+'%')
		and 1 = case when @WarehouseCode = 'ALL' or @WarehouseCode = ml.WarehouseCode then 1 else 0 end
	)

	declare @sql varchar(max) = 
	'
		select 
			ml.WarehouseCode, mw.WH_Name WarehouseName, ml.AreaCode, ml.AreaName,
			ml.RegisterDate, us.FullName RegisterUser, ml.UpdateDate LastUpdate, us2.FullName LastUser, '''+cast(@TotalRows as varchar)+''' TotalRows
		From MS_Area ml
		left join WareHouse_Master mw on mw.WH_Code = ml.WarehouseCode
		left join vw_User us on ml.RegisterBy = us.UserID
		left join vw_User us2 on ml.UpdateBy = us2.UserID
		where 1=1
		and (ml.AreaCode like ''%'+@Keyword+'%'' or ml.AreaName like ''%'+@Keyword+'%'')
		and 1 = case when '''+ @WarehouseCode +''' = ''ALL'' or '''+ @WarehouseCode +''' = ml.WarehouseCode then 1 else 0 end
		'+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)

end
GO
