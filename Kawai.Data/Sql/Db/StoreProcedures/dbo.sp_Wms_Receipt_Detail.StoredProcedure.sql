SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_Receipt_Detail]
	@ReceiptId bigint
as
begin
	SELECT
		a.Id,
		a.ReceiptNo, 
		a.ReceiptDate,
		a.DNNumber,
		a.SupplierCode,
		b.Trade_Name AS SupplierName,
		a.DNDate,
		a.BCNumber,
		a.BCType,
		a.BCDate,
		a.VehicleNo,
		a.LastUpdate,
		c.FullName LastUser
	FROM PartReceiptHeader a
	LEFT JOIN trade_master b ON a.SupplierCode = b.Trade_Code
	left join ss_usersetup c on isnull(a.LastUser, a.RegisterUser) = c.UserID
	WHERE a.Id = @ReceiptId
end
GO
