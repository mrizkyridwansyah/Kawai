SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [sp_Wms_DeliveryPlace_List]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',
	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@Trade_Code varchar(25)=''
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
		set @sqlSort = 'order by wh.Location_Code'
	end 

	declare @TotalRows int =
	(
		select count(1)
		From Delivery_Place wh
		left join vw_User us on wh.Last_user = us.UserID
	 	where 1=1 and Trade_Code = @Trade_Code
		and (wh.Location_Code like '%'+@Keyword+'%' or wh.Location_Name like '%'+@Keyword+'%')
	)

	declare @sql varchar(max) = 
	'
		select 
			wh.Trade_Code,wh.Location_Code, wh.Location_Name,wh.Last_Update, us.FullName Last_User,wh.Register_Date, 
			'''+cast(@TotalRows as varchar)+''' as TotalRows
		From Delivery_Place wh
		left join vw_User us on wh.Last_user = us.UserID
	 	where 1=1 and wh.Trade_Code = '''+ @Trade_Code +'''
		and (wh.Location_Code like ''%'+@Keyword+'%'' or wh.Location_Name like ''%'+@Keyword+'%'')
		'+ @sqlSort +'
		OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
		FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
	'

	print @sql

	execute (@sql)

end
GO
