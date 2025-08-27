SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_Receipt_ListDetail]
	@ReceiptId bigint
as
begin
	SELECT
		a.Id, a.ReceiptId,
		a.PONumber, 
		a.ItemCode,
		b.Item_Name [ItemName],
		a.UnitCls [UnitClsCode],
		c.Description [UnitClsName],
		a.ExpectedQty, a.TotalPacking, a.ReceiptQty, a.IQCResult
	FROM PartReceiptDetail a
	LEFT JOIN Item_Master b ON a.ItemCode = b.Item_Code
	LEFT JOIN Unit_Cls c ON a.UnitCls = c.Unit_Cls
	WHERE a.ReceiptId = @ReceiptId
	and isnull(a.HasValid, 0) = 1
end
GO
