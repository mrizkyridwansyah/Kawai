SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_NGClaimMaterial_ListPODetail]
--Declare
 	@ClaimId bigint = 4,
	@SupplierCode varchar(25)='S0059',
	@DateFrom datetime ='2025/12/01' ,
	@DateUntil datetime='2025/12/26' 
as
begin
		
	begin
		IF EXISTS (select TOP 1 1 F from MaterialNGClaimHeader where ClaimID = @ClaimId)
		 BEGIN

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
	   UNION ALL
		SELECT 
			rcp.ClaimID [ClaimID],
			rcp.DetailID [DetailID],	
			a.PO_Number as PONumber,
            a.ReceiptNo as ReceiptNumber,
			ss.ReceiptDate,
 
			a.SupplierCode [SupplierCode],
			c.Trade_Name [SupplierName],
			a.ItemCode [ItemCode],
			d.Item_Name [ItemName],
			d.Unit_Cls [UnitClsCode],
			e.[Description] [UnitClsName],
			b.SampleQTY [Qty],
			b.NGCode,
			 dd.[Description] as NGDescs,
			a.LastUpdate [LastUpdate],
			us.FullName [LastUser]
		FROM IQC_Inspection_Header a
		inner join IQC_SamplingBarcodeDetail b on a.InspectionID = b.InspectionID
		left JOIN PartReceiptHeader ss on a.ReceiptNo = ss.ReceiptNo and ss.SupplierCode = a.SupplierCode
		left join trade_master c on a.SupplierCode = c.Trade_Code
		left join Item_Master d on a.ItemCode = d.Item_Code
		Left JOIN MS_NG dd on dd.NGCode = b.NGCode
		left join Unit_Cls e on d.Unit_Cls = e.Unit_Cls
	 	left join 
		(
			select * From MaterialNGClaimDetail where ClaimID = @ClaimID
		) rcp on a.PO_Number = rcp.PONumber and a.ItemCode = rcp.ItemCode and a.ReceiptNo = rcp.ReceiptNo
		left join vw_User us on b.RegisterUser = us.UserID
		WHERE 1=1 
		 and a.SupplierCode = @SupplierCode
		and a.Soruce = 'Material NG' and a.InspectionResult = 'Rejected' 
		 and Not EXISTS(
		 select SupplierCode,ItemCode , PONumber , ReceiptNo, NGCode from 
		 MaterialNGClaimHeader AA 
		 LEFT JOIN MaterialNGClaimDetail BB ON AA.ClaimID = BB.ClaimID
		 where AA.SupplierCode = @SupplierCode and BB.PONumber  = a.PO_Number and BB.ReceiptNo = a.ReceiptNo and BB.ItemCode = a.ItemCode and B.NGCode = BB.NGCode)
	 END
	 ELSE
	 BEGIN
	 	SELECT 
			rcp.ClaimID [ClaimID],
			rcp.DetailID [DetailID],	
			a.PO_Number as PONumber,
            a.ReceiptNo as ReceiptNumber,
			ss.ReceiptDate,
 
			a.SupplierCode [SupplierCode],
			c.Trade_Name [SupplierName],
			a.ItemCode [ItemCode],
			d.Item_Name [ItemName],
			d.Unit_Cls [UnitClsCode],
			e.[Description] [UnitClsName],
			b.SampleQTY [Qty],
			b.NGCode,
			 dd.[Description] as NGDescs,
			a.LastUpdate [LastUpdate],
			us.FullName [LastUser]
		FROM IQC_Inspection_Header a
		inner join IQC_SamplingBarcodeDetail b on a.InspectionID = b.InspectionID
		left JOIN PartReceiptHeader ss on a.ReceiptNo = ss.ReceiptNo and ss.SupplierCode = a.SupplierCode
		left join trade_master c on a.SupplierCode = c.Trade_Code
		left join Item_Master d on a.ItemCode = d.Item_Code
		Left JOIN MS_NG dd on dd.NGCode = b.NGCode
		left join Unit_Cls e on d.Unit_Cls = e.Unit_Cls
	 	left join 
		(
			select * From MaterialNGClaimDetail where ClaimID = @ClaimID
		) rcp on a.PO_Number = rcp.PONumber and a.ItemCode = rcp.ItemCode and a.ReceiptNo = rcp.ReceiptNo
		left join vw_User us on b.RegisterUser = us.UserID
		WHERE 1=1 
		 and a.SupplierCode = @SupplierCode
		and a.Soruce = 'Material NG' and a.InspectionResult = 'Rejected' 
		 and Not EXISTS(
		 select SupplierCode,ItemCode , PONumber , ReceiptNo, NGCode from 
		 MaterialNGClaimHeader AA 
		 LEFT JOIN MaterialNGClaimDetail BB ON AA.ClaimID = BB.ClaimID
		 where AA.SupplierCode = @SupplierCode and BB.PONumber  = a.PO_Number and BB.ReceiptNo = a.ReceiptNo and BB.ItemCode = a.ItemCode and B.NGCode = BB.NGCode)
	
	 END
	end	
end
GO
