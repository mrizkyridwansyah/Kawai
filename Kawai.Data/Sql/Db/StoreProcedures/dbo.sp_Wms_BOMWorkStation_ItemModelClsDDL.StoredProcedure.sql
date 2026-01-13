SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [sp_Wms_BOMWorkStation_ItemModelClsDDL]
	@Keyword varchar(max),
	@ModelCls varchar(10)
as
	select
	   Item_Code as ItemCode
      ,RTRIM(Item_Name) as ItemName,
	    Item_Code +' | '+ RTRIM(Item_Name) DDLDescription
 
	From Item_Master
	where 1=1
	and (@ModelCls = 'ALL' or Model_Cls = @ModelCls)
	and (Item_Code like '%' + @Keyword + '%' or Item_Name like '%' + @Keyword + '%')
GO
