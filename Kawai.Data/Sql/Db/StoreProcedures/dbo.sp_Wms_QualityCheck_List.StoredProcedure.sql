SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE   procedure [sp_Wms_QualityCheck_List]
	-- PARAMETER WAJIB
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	@Keyword varchar(max) = '',
	@Trade_Code varchar(max) = '',
	@DN_No varchar(max) = '',
    @Item_Code varchar(max) = '',
	@QC_Status varchar(max) = '',
	@ReceiptFromData varchar(10)='',
	@ReceiptFromTo varchar(10)=''
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
		set @sqlSort = 'order by qc.DN_No'
	end 

	
	declare @TotalRow int = 
	(
		SELECT Count(1)  
		FROM Quality_Check qc
		LEFT JOIN Item_Master im ON im.Item_code = qc.item_code
		LEFT JOIN Trade_Master tm ON tm.Trade_Code = qc.Supplier_Code
		LEFT JOIN SS_UserSetup us ON qc.Last_Update = us.UserID
		LEFT JOIN SS_UserSetup us2 ON qc.Inspection_User = us2.UserID
		WHERE 1=1
		AND (@Keyword IS NULL OR qc.DN_No LIKE '%' + @Keyword + '%' OR qc.Item_Code LIKE '%' + @Keyword + '%'OR im.Item_Name LIKE '%' + @Keyword + '%' OR us2.FullName  LIKE '%' + @Keyword + '%')
		AND (@Trade_Code IS NULL OR qc.Supplier_Code = @Trade_Code)
		AND (@DN_No IS NULL OR qc.DN_No = @DN_No)
		AND (@Item_Code IS NULL OR qc.Item_Code = @Item_Code)
		AND (@QC_Status IS NULL OR qc.QC_Status = @QC_Status)
		 
	)

	DECLARE @sql NVARCHAR(MAX) = N'
	  
		SELECT 
		qc.DN_No
        ,qc.Item_Code
		,im.Item_Name
        ,qc.Supplier_Code
        ,qc.Receipt_Date
        ,qc.Inspection_Date
        ,qc.Inspection_User
        ,qc.QC_Status
        ,qc.QC_Photo
        ,qc.Remarks
        ,qc.Status
        ,qc.Last_Update
        ,qc.Last_User
        ,qc.Register_Date
        ,qc.Qty
		,'''+cast(@TotalRow as varchar)+''' as TotalRows
		FROM Quality_Check qc
		LEFT JOIN Item_Master im ON im.Item_code = qc.item_code
		LEFT JOIN Trade_Master tm ON tm.Trade_Code = qc.Supplier_Code
		LEFT JOIN SS_UserSetup us ON qc.Last_Update = us.UserID
		LEFT JOIN SS_UserSetup us2 ON qc.Inspection_User = us2.UserID
		WHERE 1=1
		AND (@Keyword IS NULL OR qc.DN_No LIKE ''%'' + @Keyword + ''%'' OR qc.Item_Code LIKE ''%'' + @Keyword + ''%''OR im.Item_Name LIKE ''%'' + @Keyword + ''%'' OR us2.FullName  LIKE ''%'' + @Keyword + ''%'')
		AND (@Trade_Code IS NULL OR qc.Supplier_Code = @Trade_Code)
		AND (@DN_No IS NULL OR qc.DN_No = @DN_No)
		AND (@Item_Code IS NULL OR qc.Item_Code = @Item_Code)
		AND (@QC_Status IS NULL OR qc.QC_Status = @QC_Status)


	  ' + @sqlSort + '
	  OFFSET @Offset ROWS FETCH NEXT @Length ROWS ONLY
	';

	EXEC sp_executesql 
	  @sql,
	  N'@Keyword VARCHAR(MAX), @Trade_Code VARCHAR(MAX), @Item_Code VARCHAR(MAX), @DN_No VARCHAR(MAX), @QC_Status VARCHAR(MAX), @Offset INT, @Length INT, @TotalRow INT',
	  @Keyword = @Keyword,
	  @Trade_Code = @Trade_Code,
	  @Item_Code = @Item_Code,
	  @DN_No = @DN_No,
	  @QC_Status = @QC_Status,
	  @Offset = @offset,
	  @Length = @Length,
	  @TotalRow = @TotalRow;

end
GO
