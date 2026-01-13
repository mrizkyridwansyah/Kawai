SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   procedure [sp_Wms_NGClaimMaterial_Capture]
	@ClaimId bigint
as

select 
ClaimID
,ClaimNo
,SupplierCode
,SupplierName
,ClaimDate
,TotalQty
,TotalAmount
,Status
,Notes
,RegisterDate
,RegisterUser
,LastUpdate
,LastUser
,DNNumber
,DNDate
,BCNumber
,BCType
,BCDate
,VehicleNo
,Transport
,Approved_User
,Approved_Date
From MaterialNGClaimHeader where ClaimID = @ClaimId

select * From MaterialNGClaimDetail where ClaimID = @ClaimId

 
GO
