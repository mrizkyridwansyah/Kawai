SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




Create   PROCEDURE [sp_Wms_Classification_Delete]
	@Code Varchar(100) = '34',
	@TableName 	 varchar(25) = 'PaymentTerm_Cls'  
as
begin
	Declare @Field_Code Varchar(100) ,@Field_Descs Varchar(100) ,@Sql Varchar(Max)
Select @Field_Code = Field_1,@Field_Descs = Field_2 from WMS_Tab_Classification where TableName = @TableName

Set @Sql = ' 
     if not exists (select 1 from '+ @TableName +' where '+ @Field_Code +' =  '''+ @Code +''')
	begin
		raiserror(''Code Not Exists'',16,1)
		return;
	end
	Delete From  '+ @TableName +'  where '+ @Field_Code +' =  '''+ @Code +'''
 	 
		  
	'
 
 exec (@sql)
 

end
GO
