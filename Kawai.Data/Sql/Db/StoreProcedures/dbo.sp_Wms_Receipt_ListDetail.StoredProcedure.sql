SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_Receipt_ListDetail]
	@ReceiptId bigint
as
begin
	declare @SupplierCode varchar(50) = (select SupplierCode from PartReceiptHeader where Id = @ReceiptId)

	SELECT
		a.Id, a.ReceiptId,
		a.PONumber, 
		a.ItemCode,
		b.Item_Name [ItemName],
		a.UnitCls [UnitClsCode],
		c.Description [UnitClsName],
		a.ExpectedQty, a.TotalPacking, a.ReceiptQty, a.IQCResult, isp.QtyPacking,
		ph.LastUpdate, us.FullName LastUser
	FROM PartReceiptDetail a
	inner join PartReceiptHeader ph on a.ReceiptId = ph.Id
	LEFT JOIN Item_Master b ON a.ItemCode = b.Item_Code
	LEFT JOIN ItemSupplierPacking isp ON a.ItemCode = isp.ItemCode and isp.SupplierCode = @SupplierCode
	LEFT JOIN Unit_Cls c ON a.UnitCls = c.Unit_Cls
	left join SS_UserSetup us on ph.LastUser = us.UserID
	WHERE a.ReceiptId = @ReceiptId
end
GO
