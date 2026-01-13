SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE  procedure [sp_Wms_NGClaimMaterial_DataHeader]
	@ClaimId bigint
as
begin
 


	 
	SELECT
		a.ClaimID,
		 a.SupplierCode
        ,a.SupplierName
        ,a.ClaimDate
        ,a.TotalQty
        ,a.TotalAmount
        ,a.Status
        ,a.Notes
        ,a.RegisterDate
        ,a.RegisterUser
        ,a.LastUpdate
        ,a.LastUser
        ,a.DNNumber
        ,a.DNDate
        ,a.BCNumber
        ,a.BCType
        ,a.BCDate
        ,a.VehicleNo
        ,a.Transport,

		 
		b.Trade_Name AS SupplierName,
		 
	a.ClaimDate [ClaimDateFrom],
		a.ClaimDate [ClaimDateTo],
		 
		a.LastUpdate,
		c.FullName LastUser
	FROM MaterialNGClaimHeader a
	LEFT JOIN trade_master b ON a.SupplierCode = b.Trade_Code
	left join ss_usersetup c on isnull(a.LastUser, a.RegisterUser) = c.UserID	
 
	WHERE a.ClaimID = @ClaimId
end
GO
