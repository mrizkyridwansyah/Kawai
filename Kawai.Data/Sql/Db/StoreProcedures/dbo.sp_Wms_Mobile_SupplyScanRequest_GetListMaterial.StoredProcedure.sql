SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE   procedure [sp_Wms_Mobile_SupplyScanRequest_GetListMaterial]
	@RequestNo varchar(50),
	@ItemClass varchar(50)
as

select  
	b.ItemCode , c.Item_Name as ItemName, b.ChildRequirement_Qty as PlanQty,  
	ISNULL((select SUM(Qty) from PartMaterialRequestItemDetailScan cc where cc.ItemCode = b.ItemCode and cc.IDSeq = B.IDSeq),0) QtyScan, 
	b.Unit_Cls, d.[Description] as UnitDesc , b.RequestDetailID ,@RequestNo RequestNo , '' WarehouseCode
From PartMaterialRequestDetail A 
LEFT JOIN PartMaterialRequestItemDetail B ON A.RequestDetailID = B.RequestDetailID
Left Join Item_Master c on b.ItemCode = c.Item_Code
Left JOIN Unit_Cls d on d.Unit_Cls = b.unit_Cls
where a.RefNumber = @RequestNo and a.AreaCode = @ItemClass
 
 
 
GO
