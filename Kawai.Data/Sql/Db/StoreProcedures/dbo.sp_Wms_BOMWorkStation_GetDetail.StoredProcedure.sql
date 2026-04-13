SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [sp_Wms_BOMWorkStation_GetDetail]
--declare 
		@LineCode Varchar(100)='011' ,
		@ParentItem_Code Varchar(100)='10072070',
		@WorkStationCode Varchar(100) 
    
as 
Declare @SeqNo int
Select @SeqNo = Bomws_ID From MS_BOMPerworkstation_Header where Line_Code = @LineCode and ParentItemCode = @ParentItem_Code and WorkStationCode = @WorkStationCode
SELECT * FROM (
 Select  
  AllowSetting	,
  TypeMaterial,
  TypeMaterialCode 
 ,ChildItem_Code	
 ,ChildItem_Name	
  ,Case when AllowSetting = 1 then QtyBOMWorkStation
      Else QtyBOM  END Qty 
 ,Unit_Cls	
 ,Unit_Descs	
 ,RegisterUser	
 ,RegisterDate	
 ,LastUser	
 ,LastUpdate
from (
Select 
ISNULL((Select Top 1 1 From MS_BOMPerworkstation_Detail B where B.Bomws_ID = @SeqNo and B.ChildItem_Code = A.Item_Code),0) AllowSetting,
A.Item_Code ChildItem_Code , 
A.Item_Name ChildItem_Name,
B.ClasificationPart_Cls TypeMaterialCode,
D.[Description] TypeMaterial,
ISNULL((Select Top 1 Qty From MS_BOMPerworkstation_Detail B where B.Bomws_ID = @SeqNo and B.ChildItem_Code = A.Item_Code),0)QtyBOMWorkStation, 
 A.Qty  - ISNULL((Select  SUM(Qty) From MS_BOMPerworkstation_Detail B Left Join MS_BOMPerworkstation_Header cc on cc.Bomws_ID = B.Bomws_ID where CC.Line_Code = @LineCode and  B.ChildItem_Code = A.Item_Code),0) as QtyBOM, 

 
 A.Unit_Cls, C.[Description] as Unit_Descs,  A.Last_User RegisterUser,
A.Register_Date RegisterDate,
 (Select Top 1 RegisterUser From MS_BOMPerworkstation_Detail B where B.Bomws_ID = @SeqNo and B.ChildItem_Code = A.Item_Code)  LastUser
,(Select Top 1 RegisterDate From MS_BOMPerworkstation_Detail B where B.Bomws_ID = @SeqNo and B.ChildItem_Code = A.Item_Code)  LastUpdate

from BOM_Master A 
LEFT JOIN Unit_Cls C ON A.Unit_Cls = C.Unit_Cls 
Left join Item_Master B ON A.item_Code = B.Item_Code
left join ClasificationPart_Cls d ON D.ClasificationPart_Cls = B.ClasificationPart_Cls
where Parent_ItemCode = @ParentItem_Code 

) A
) A  where Qty > 0.00000 order by AllowSetting Desc , TypeMaterialCode Asc
 
GO
