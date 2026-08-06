CREATE PROCEDURE [dbo].[sp_Wms_Shipping_Instruction_List_Picking]
	-- PARAMETER WAJIB

	 --Declare
	@Page int = 1,
	@Length int = 10,
	@Sort varchar(max) = '',

	-- PARAMETER OPSIONAL
	 @Keyword varchar(max) = '',
	 @ShippingInstructionNo  varchar(100)='SI-KI3-eee',
     @ItemCode  varchar(100)='100T3171',
     @PONumber  varchar(100)='KI3-14968' ,
     @PO_SeqNo int= 1
	 
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
		set @sqlSort = 'order by SerialNo'
	end 

	
	    declare @TotalRows int 
	 
		 
		 IF EXISTS (Select Top 1 1 From ShippingInstruction_Detail where SI_No =@ShippingInstructionNo)
		 BEGIN
		    Select @TotalRows = Count(1) From ShippingInstruction_Detail 
			where 1=1 
			AND ( @Keyword = '' OR Serial_No LIKE '%' + @Keyword + '%'  ) AND
		    SI_No = @ShippingInstructionNo and PO_NO = @PONumber and PO_SeqNo = @PO_SeqNo and Item_Code = @ItemCode
		 END
		 ELSE
		 BEGIN 
		 Declare @SerialFrom Varchar(30),  @SerialTo Varchar(30)
		  select @SerialFrom = SerialNoFrom , @SerialTo = SerialNoto from OrderEntry_Detail where PO_No = @PONumber and Seq_No = @PO_SeqNo and Item_Code = @ItemCode
		  select @TotalRows = Count(1) from Serial_Detail where   1=1 
			AND ( @Keyword = '' OR Serial_No LIKE '%' + @Keyword + '%'  ) AND Serial_No between @SerialFrom and @SerialTo
         END
 
		 
	 

	declare @sql varchar(max) = 
	'
	 IF EXISTS (Select Top 1 1 From ShippingInstruction_Detail where SI_No = '''+ CAST( @ShippingInstructionNo as Varchar) + ''' )
		 BEGIN
		  Select * from (
				Select CASE WHEN ISNULL(IsPicking,''0'') =''0'' then 0 else 1 end AlreadyPicking, Serial_No as SerialNo , cast(Picking_Date as date) PickingDate ,FORMAT(Picking_Date,''HH:mm:sss'') PickingTime    ,Picking_By PickingBy ,'''+cast(@TotalRows as varchar)+''' TotalRows 
				From ShippingInstruction_Detail 
				where 1=1 
				AND ( '''+  @Keyword + '''  = '''' OR Serial_No LIKE ''%' + @Keyword + '%''  ) AND
				SI_No = '''+ CAST(@ShippingInstructionNo as Varchar) + '''   and PO_NO = '''+ CAST( @PONumber as Varchar) + '''  and PO_SeqNo = '''+ CAST(@PO_SeqNo as Varchar) + '''   and Item_Code = '''+ CAST( @ItemCode as Varchar) + ''' 
		    ) A '+ @sqlSort +'
			OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
			FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
		 END
		 ELSE
		 BEGIN 
		 Declare @SerialFrom Varchar(30),  @SerialTo Varchar(30)
		  select @SerialFrom = SerialNoFrom , @SerialTo = SerialNoto from OrderEntry_Detail where PO_No ='''+ CAST( @PONumber as Varchar) + ''' and Seq_No = '''+ CAST(@PO_SeqNo as Varchar) + ''' and Item_Code = '''+ CAST( @ItemCode as Varchar) + '''
		  Select * from (
					  select 0 AlreadyPicking, Serial_No as SerialNo , NULL PickingDate ,NULL PickingTime ,     NULL PickingBy , '''+cast(@TotalRows as varchar)+''' TotalRows  from Serial_Detail where   1=1 
					  AND ( '''+  @Keyword + '''  = '''' OR Serial_No LIKE ''%' + @Keyword + '%''  ) AND Serial_No between @SerialFrom and @SerialTo
			 ) A
			'+ @sqlSort +'
			OFFSET ' + cast(@offset as varchar(10)) + ' ROWS 
			FETCH NEXT ' + cast(@Length as varchar(10)) + ' ROWS ONLY
		 END


	 	 
	'

	print @sql

	execute (@sql)


