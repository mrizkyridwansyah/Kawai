SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [sp_Wms_BOMWorkStation_List]
--declare
-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@Line Varchar(100) ,
    @ModelCls Varchar(100),
	@ItemCode Varchar(100) 
  as

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
		set @sqlSort = 'order by wh.WorkStationCode'
	end 

		declare @TotalRows int =
		(
			select count(1)
			   From WorkStationLineSetting wh
		      left join MS_WorkStation ws on wh.WorkStationCode = ws.WorkStationCode
			where 1=1
			and (wh.WorkStationCode like '%'+@Keyword+'%' or ws.WorkStationName like '%'+@Keyword+'%')
			and 1 = case when @Line = 'ALL' or @Line = wh.LineCode then 1 else 0 end
		 
		)

	declare @sql varchar(max) 
	
	Begin
	set  @sql = 
	'
		select 
			 wh.WorkStationCode
            ,ws.WorkStationName
			,(select ff.Description from MS_BOMPerworkstation_Header zz left join Trolley_Cls ff ON zz.Troly_Cls = ff.Trolley_Cls  where zz.ParentItemCode = '''+ @ItemCode +''' and  zz.Line_Code = wh.LineCode and zz.WorkStationCode = wh.WorkStationCode) TrolleyCls
			,(select zz.MAX_Qty_Set from MS_BOMPerworkstation_Header zz  where zz.ParentItemCode = '''+ @ItemCode +''' and  zz.Line_Code = wh.LineCode and zz.WorkStationCode = wh.WorkStationCode) MaxQtySet
            ,wh.RegisterDate
			,wh.RegisterUser
		    ,(select zz.LastUpdate from MS_BOMPerworkstation_Header zz  where zz.ParentItemCode = '''+ @ItemCode +''' and  zz.Line_Code = wh.LineCode and zz.WorkStationCode = wh.WorkStationCode) LastUpdate
			,(select zz.LastUser from MS_BOMPerworkstation_Header zz  where zz.ParentItemCode = '''+ @ItemCode +''' and  zz.Line_Code = wh.LineCode and zz.WorkStationCode = wh.WorkStationCode) LastUser, 
			'''+cast(@TotalRows as varchar)+''' as TotalRows
		From WorkStationLineSetting wh
		left join MS_WorkStation ws on wh.WorkStationCode = ws.WorkStationCode
		left join vw_User us on wh.LastUser = us.UserID
		 	where 1=1
		and (wh.WorkStationCode like ''%'+@Keyword+'%'' or ws.WorkStationName like ''%'+@Keyword+'%'')
		and 1 = case when '''+ @Line +''' = ''ALL'' or '''+ @Line +''' = wh.LineCode then 1 else 0 end
		
		 '+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'
	End
	print @sql

	execute (@sql)

 

GO
