SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE   procedure [sp_Wms_Address_List]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@WarehouseCode varchar(max),
	@AreaCode varchar(max)
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
		set @sqlSort = 'order by ma.AddressCode'
	end 

	
	declare @TotalRow int = 
	(
		SELECT COUNT(1) AS TotalRow
		FROM MS_Address ma
		LEFT JOIN WareHouse_Master mw ON mw.WH_Code = ma.WarehouseCode
		LEFT JOIN MS_Area ml ON ml.AreaCode = ma.AreaCode
		LEFT JOIN SS_UserSetup us ON ma.RegisterBy = us.UserID
		LEFT JOIN SS_UserSetup us2 ON ma.UpdateBy = us2.UserID
		WHERE 1=1
		AND (@Keyword IS NULL OR ma.AddressCode LIKE '%' + @Keyword + '%' OR ma.AddressName LIKE '%' + @Keyword + '%')
		AND (@WarehouseCode IS NULL OR ma.WarehouseCode = @WarehouseCode)
		AND (@AreaCode IS NULL OR ma.AreaCode = @AreaCode)	
	)

	DECLARE @sql NVARCHAR(MAX) = N'
	  SELECT 
		ma.WarehouseCode, mw.WH_Name WarehouseName, ma.AreaCode, ml.AreaName, ma.AddressCode, ma.AddressName,
		ma.RegisterDate, us.FullName RegisterUser, ma.UpdateDate LastUpdate, us2.FullName LastUser, TotalRow = @TotalRow
	  FROM MS_Address ma
	  LEFT JOIN WareHouse_Master mw ON mw.WH_Code = ma.WarehouseCode
	  LEFT JOIN MS_Area ml ON ml.AreaCode = ma.AreaCode
	  LEFT JOIN SS_UserSetup us ON ma.RegisterBy = us.UserID
	  LEFT JOIN SS_UserSetup us2 ON ma.UpdateBy = us2.UserID
	  WHERE 1=1
		AND (@Keyword IS NULL OR ma.AddressCode LIKE ''%'' + @Keyword + ''%'' OR ma.AddressName LIKE ''%'' + @Keyword + ''%'')
		AND (@WarehouseCode IS NULL OR ma.WarehouseCode = @WarehouseCode)
		AND (@AreaCode IS NULL OR ma.AreaCode = @AreaCode)
	  ' + @sqlSort + '
	  OFFSET @Offset ROWS FETCH NEXT @Length ROWS ONLY
	';

	EXEC sp_executesql 
	  @sql,
	  N'@Keyword VARCHAR(MAX), @WarehouseCode VARCHAR(MAX), @AreaCode VARCHAR(MAX), @Offset INT, @Length INT, @TotalRow INT',
	  @Keyword = @Keyword,
	  @WarehouseCode = @WarehouseCode,
	  @AreaCode = @AreaCode,
	  @Offset = @offset,
	  @Length = @Length,
	  @TotalRow = @TotalRow;

end
GO
