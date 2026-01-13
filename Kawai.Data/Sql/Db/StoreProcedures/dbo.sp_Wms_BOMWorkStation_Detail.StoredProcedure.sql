SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [sp_Wms_BOMWorkStation_Detail]
--Declare
	@ParentItem_Code 	 varchar(25) = '10072071',
    @WorkStationCode varchar(25)='WS001'
as 


Select ISNULL((Select Top 1 1 From MS_BOMPerworkstation B where B.ParentItem_Code  = @ParentItem_Code  and B.WorkStationCode = @WorkStationCode and B.ChildItem_Code = A.Item_Code),0) AllowSetting,
Item_Code ChildItem_Code , Item_Name ChildItem_Name,
 
  
ISNULL((Select Top 1 Qty From MS_BOMPerworkstation B where B.ParentItem_Code  = @ParentItem_Code  and B.WorkStationCode = @WorkStationCode and B.ChildItem_Code = A.Item_Code),(select Qty from BOM_Master DD where DD.Parent_ItemCode = @ParentItem_Code and DD.Item_Code = A.Item_Code) - ISNULL((Select Top 1 Qty From MS_BOMPerworkstation B where B.ParentItem_Code  = @ParentItem_Code  and B.WorkStationCode <> @WorkStationCode and B.ChildItem_Code = A.Item_Code),0)) Qty,
 A.Unit_Cls, C.[Description] as Unit_Descs,  A.Last_User RegisterUser,
A.Register_Date RegisterDate,
 (Select Top 1 RegisterUser From MS_BOMPerworkstation B where B.ParentItem_Code  = @ParentItem_Code  and B.WorkStationCode = @WorkStationCode and B.ChildItem_Code = A.Item_Code)  LastUser
,(Select Top 1 RegisterDate From MS_BOMPerworkstation B where B.ParentItem_Code  = @ParentItem_Code  and B.WorkStationCode = @WorkStationCode and B.ChildItem_Code = A.Item_Code)  LastUpdate

from BOM_Master A 
LEFT JOIN Unit_Cls C ON A.Unit_Cls = C.Unit_Cls where Parent_ItemCode = @ParentItem_Code 


 
GO
