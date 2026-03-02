SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [sp_Wms_Warehouse_List]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',
	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@FactoryCode varchar(max)
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
		set @sqlSort = 'order by wh.WH_Code'
	end 

	declare @TotalRows int =
	(
		select count(1)
		From WareHouse_Master wh
		left join vw_User us on wh.Last_User = us.UserID
		left join Trade_Master tm on wh.Adm_Group = tm.Trade_Code
		where 1=1
		and (wh.WH_Code like '%'+@Keyword+'%' or wh.WH_Name like '%'+@Keyword+'%')
		and 1 = case when @FactoryCode = 'ALL' or @FactoryCode = wh.Company_Code then 1 else 0 end
	)

	declare @sql varchar(max) = 
	'
		select 
			wh.Company_Code FactoryCode, cp.Company_Name FactoryName, 
			wh.WH_Code WarehouseCode, wh.WH_Name WarehouseName, wh.Adm_Group AdmGroup, tm.Trade_Name AdmGroupName,
			wh.StockControl_Cls StockControlCls, wh.NG_Cls NGCls, 
			dbo.ConvertToDateTimeFromFuckingString(wh.Use_EndDay) UseEndDate, isnull(wh.Last_Update, wh.Register_Date) LastUpdate, us.FullName Lastuser, 
			'''+cast(@TotalRows as varchar)+''' as TotalRows
		From WareHouse_Master wh
		left join vw_User us on wh.Last_User = us.UserID
		left join Trade_Master tm on wh.Adm_Group = tm.Trade_Code
		left join Company_Profile cp on wh.Company_Code = cp.Company_Code
		where 1=1
		and (wh.WH_Code like ''%'+@Keyword+'%'' or wh.WH_Name like ''%'+@Keyword+'%'' or wh.Adm_Group like ''%'+@Keyword+'%'' or tm.Trade_Name like ''%'+@Keyword+'%'')
		and 1 = case when '''+ @FactoryCode +''' = ''ALL'' or '''+ @FactoryCode +''' = wh.Company_Code then 1 else 0 end
		'+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)

end
GO
