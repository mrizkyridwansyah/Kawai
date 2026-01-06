SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_ItemPackingSupplier_GetDetail]
	@SupplierCode varchar(15),
	@ItemCode varchar(25)
as
begin
	select 
		SupplierCode, b.Trade_Name SupplierName, ItemCode, c.Item_Name ItemName, QtyPacking, LastUpdate = isnull(a.LastUpdate, a.RegisterDate), 
		d.FullName LastUser, a.UnitCls UnitClsCode, e.Description UnitClsName
	From ItemSupplierPacking a
	inner join Trade_Master b on a.SupplierCode = b.Trade_Code
	inner join Item_Master c on a.ItemCode = c.Item_Code
	inner join vw_User d on isnull(a.LastUser, a.RegisterUser) = d.UserID
	left join Unit_Cls e on c.Unit_Cls = e.Unit_Cls
	where ItemCode = @ItemCode and SupplierCode = @SupplierCode
end
GO
