SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Proc [sp_Wms_WorkStationSetting_List]
--declare
-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
    @LineCode Varchar(100) = ''
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
		set @sqlSort = 'order by  WorkStationCode'
	end 

	declare @sql varchar(max) 
	if @LineCode = ''
	begin
	
		set  @sql = 
		'
			select 
				ISNULL((Select Top 1 1 From WorkStationLineSetting B where B.LineCode = '''+ @LineCode +''' and B.WorkStationCode = A.WorkStationCode),0) AllowSetting,   
				A.WorkStationCode,A.WorkStationName,
				(Select Top 1 StopPointCode From WorkStationLineSetting B where B.LineCode = '''+ @LineCode +''' and B.WorkStationCode = A.WorkStationCode)  StopPointCode,
				A.RegisterUser , 
				A.RegisterDate ,
				(Select Top 1 RegisterUser From WorkStationLineSetting B where B.LineCode = '''+ @LineCode +''' and B.WorkStationCode = A.WorkStationCode) LastUser,
				(Select Top 1 RegisterDate From WorkStationLineSetting B where B.LineCode = '''+ @LineCode +''' and B.WorkStationCode = A.WorkStationCode)  LastUpdate,
				(Select Top 1 Barcode From WorkStationLineSetting B where B.LineCode = '''+ @LineCode +''' and B.WorkStationCode = A.WorkStationCode)  Barcode
			from MS_WorkStation A    
			where 1=0
			and (A.WorkStationCode like ''%'+@Keyword+'%'' or A.WorkStationName like ''%'+@Keyword+'%'')
			 '+ @sqlSort +'		 
		'
	end
	Else
	Begin
		set  @sql = 
		'
			select 
				ISNULL((Select Top 1 1 From WorkStationLineSetting B where B.LineCode = '''+ @LineCode +''' and B.WorkStationCode = A.WorkStationCode),0) AllowSetting,   
				A.WorkStationCode,
				A.WorkStationName,
				(Select Top 1 StopPointCode From WorkStationLineSetting B where B.LineCode = '''+ @LineCode +''' and B.WorkStationCode = A.WorkStationCode)  StopPointCode,
				A.RegisterUser , 
				A.RegisterDate ,
				(Select Top 1 RegisterUser From WorkStationLineSetting B where B.LineCode = '''+ @LineCode +''' and B.WorkStationCode = A.WorkStationCode) LastUser,
				(Select Top 1 RegisterDate From WorkStationLineSetting B where B.LineCode = '''+ @LineCode +''' and B.WorkStationCode = A.WorkStationCode)  LastUpdate,
				(Select Top 1 Barcode From WorkStationLineSetting B where B.LineCode = '''+ @LineCode +''' and B.WorkStationCode = A.WorkStationCode)  Barcode
			from MS_WorkStation A    
			where 1=1
			and (A.WorkStationCode like ''%'+@Keyword+'%'' or A.WorkStationName like ''%'+@Keyword+'%'')		
			 '+ @sqlSort +'		 
		'
	End
	print @sql

	execute (@sql)
GO
