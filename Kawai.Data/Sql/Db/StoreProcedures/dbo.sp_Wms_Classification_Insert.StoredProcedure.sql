SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [sp_Wms_Classification_Insert]
--Declare
    @Code Varchar(100) = '34',
	@Description Varchar(200) = 'test data',
	@TableName 	 varchar(25) = 'PaymentTerm_Cls' 
     
as 

Declare @Field_Code Varchar(100) ,@Field_Descs Varchar(100) ,@Sql Varchar(Max)
Select @Field_Code = Field_1,@Field_Descs = Field_2 from WMS_Tab_Classification where TableName = @TableName

Set @Sql = ' 
     if exists (select 1 from '+ @TableName +' where '+ @Field_Code +' =  '''+ @Code +''')
	begin
		raiserror(''Code Already Exists'',16,1)
		return;
	end
	
	Insert into  '+ @TableName +' (  '+ @Field_Code +' , '+ @Field_Descs +') Values ( '''+ @Code +''' , '''+ @Description +''')
	 
		  
	'
 
 exec (@sql)
 
GO
