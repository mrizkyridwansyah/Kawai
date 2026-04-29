CREATE procedure [dbo].[sp_Wms_NGClaimMaterial_ListNGClaimDetail]
--Declare
 	@ClaimId bigint = N'20004',
	@SupplierCode varchar(25)='CSS005',
	@DateFrom datetime ='2026-04-01' ,
	@DateUntil datetime='2026-04-01'  
as
begin
		
	begin
		
		 
		SELECT 
			a.ClaimID [ClaimID],
			b.DetailID [DetailID],	
			b.PONumber as PONumber,
            b.ReceiptNo as ReceiptNumber,
			ss.ReceiptDate,
 
			a.SupplierCode [SupplierCode],
			c.Trade_Name [SupplierName],
			b.ItemCode [ItemCode],
			d.Item_Name [ItemName],
			d.Unit_Cls [UnitClsCode],
			e.[Description] [UnitClsName],
			b.QtyNG [Qty],
			b.NGCode,
			 dd.[Description] as NGDescs,
			a.LastUpdate [LastUpdate],
			us.FullName [LastUser]
		FROM MaterialNGClaimHeader a
		inner join MaterialNGClaimDetail b on a.ClaimID = b.ClaimID
		left JOIN PartReceiptHeader ss on b.ReceiptNo = ss.ReceiptNo and ss.SupplierCode = a.SupplierCode
		left join trade_master c on a.SupplierCode = c.Trade_Code
		left join Item_Master d on b.ItemCode = d.Item_Code
		Left JOIN MS_NG dd on dd.NGCode = b.NGCode
		left join Unit_Cls e on d.Unit_Cls = e.Unit_Cls
	 	left join vw_User us on b.RegisterUser = us.UserID
		WHERE 1=1 
		 and a.SupplierCode = @SupplierCode and a.ClaimID = @ClaimId
		 	end	
end

 
