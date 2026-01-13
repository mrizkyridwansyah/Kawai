SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [sp_Wms_NGClaimMaterial_ListDetail]
	@ClaimId bigint
as
begin
 
	SELECT a.ClaimID , a.DetailID,
		a.PONumber, 
		a.ReceiptNo,
		ss.ReceiptDate,
		a.ItemCode,
		b.Item_Name [ItemName],
		b.Unit_Cls [UnitClsCode],
		c.Description [UnitClsName],
		a.QtyNG [Qty],
		 a.NGCode,
		 dd.[Description] as NGDescs,
		 ph.LastUpdate, us.FullName LastUser
	FROM MaterialNGClaimDetail a
	inner join MaterialNGClaimHeader ph on a.ClaimID = ph.ClaimID
	left JOIN PartReceiptHeader ss on a.ReceiptNo = ss.ReceiptNo and ss.SupplierCode = ph.SupplierCode
	LEFT JOIN Item_Master b ON a.ItemCode = b.Item_Code
	 LEFT JOIN Unit_Cls c ON b.Unit_Cls = c.Unit_Cls
	 Left JOIN MS_NG dd on dd.NGCode = a.NGCode
	left join SS_UserSetup us on ph.LastUser = us.UserID
	WHERE a.ClaimID = @ClaimId
end
GO
