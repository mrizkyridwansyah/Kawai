SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE   PROCEDURE [sp_Wms_Classification_Update]
    @Code Varchar(100) = '34',
	@Description Varchar(200) = 'test data',
	@TableName 	 varchar(25) = 'PaymentTerm_Cls' ,
	@UpdateBy varchar(25)
as

Declare @Field_Code Varchar(100) ,@Field_Descs Varchar(100) ,@Sql Varchar(Max)
Select @Field_Code = Field_1,@Field_Descs = Field_2 from WMS_Tab_Classification where TableName = @TableName

Set @Sql = ' 
     if not exists (select 1 from '+ @TableName +' where '+ @Field_Code +' =  '''+ @Code +''')
	begin
		raiserror(''Code Not Exists'',16,1)
		return;
	end
	Declare @LengthData Int , @LengthDataCode int,  @Msg Varchar(max) = ''''
		  SELECT 
		  @LengthData = CHARACTER_MAXIMUM_LENGTH
		  FROM INFORMATION_SCHEMA.COLUMNS
		  WHERE TABLE_NAME = '''+ @TableName +'''
		  AND COLUMN_NAME = '''+ @Field_Code +''';
    
		 Set @LengthDataCode = Len('''+ @Code +''')
		 If (@LengthDataCode > @LengthData)
		 Begin
			set @Msg = '''+ @Field_Code +' Code tidak boleh lebih dari ''+Cast( @LengthData as varchar)+'' karakter''
			raiserror( @Msg,16,1)
			return;
		 End

		 SELECT 
		  @LengthData = CHARACTER_MAXIMUM_LENGTH
		  FROM INFORMATION_SCHEMA.COLUMNS
		  WHERE TABLE_NAME = '''+ @TableName +'''
		  AND COLUMN_NAME = '''+ @Field_Descs +''';
    
		 Set @LengthDataCode = Len('''+ @Description +''')
		 If (@LengthDataCode > @LengthData)
		 Begin
			set @Msg = '''+ @Field_Code +' Description tidak boleh lebih dari ''+Cast( @LengthData as varchar)+'' karakter''
			raiserror( @Msg,16,1)
			return;
		 End

	Update '+ @TableName +'  set  '+ @Field_Code +' =  '''+ @Code +''' ,  '+ @Field_Descs +' = '''+ @Description +''' where '+ @Field_Code +' =  '''+ @Code +'''
 	 
		  
	'
 
 exec (@sql)
 

 
GO
