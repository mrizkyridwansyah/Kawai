SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [sp_Wms_Classification_TableListDetail]
--Declare
	@TableName 	 varchar(25) = 'PaymentTerm_Cls' 
     
as 

Declare @Field_Code Varchar(100) ,@Field_Descs Varchar(100) ,@Sql Varchar(Max)
Select @Field_Code = Field_1,@Field_Descs = Field_2 from WMS_Tab_Classification where TableName = @TableName

Set @Sql = '
		select 
			'+ @Field_Code +' Code,
			'+ @Field_Descs +' Description
		From '+ @TableName +'  
		  
	'
 
 execute (@sql)
 
GO
