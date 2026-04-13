SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create   procedure [sp_Wms_Mobile_SupplySubcon_DDLRequestNo]
--declare
	@Keyword varchar(max)='',
	@ClassificationCode varchar(25)  ,
	@SupplierCode varchar(25)  
as
begin
	select RequestNo, [Description] from 
	(
		select
			ROW_NUMBER() OVER (ORDER BY a.RequestId, bb.SEQ) SetNumber,
			bb.RefNumber RequestNo,   [Description] = 'PO Date : ' + convert(varchar, a.ProductionDate, 113) + ', ' + rtrim(a.PO_NO) + ', '+ cls.Description
		From PartMaterialRequestHeader_PO a 
		inner join PurchaseOrder_Master po on a.PO_NO = po.PO_No
		inner join PartMaterialRequestDetail_PO bb on bb.RequestID = a.RequestID 
		inner join ClasificationPart_Cls cls on bb.AreaCode = cls.ClasificationPart_Cls
		where 1=1 and bb.RequestStatusID <> 5
		and (@ClassificationCode = 'ALL' or AreaCode = @ClassificationCode)
		and po.Supplier_Code = @SupplierCode
		and bb.RefNumber like '%' + @Keyword + '%'
	) res
	--where SetNumber = 1
end 
GO
