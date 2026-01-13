SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create   procedure [sp_Wms_PartMaterialRequestBom_Delete]
	@RequestId bigint,
	@UserId varchar(25)
as
begin
	delete pmrid 
	From PartMaterialRequestItemDetail_PO pmrid
	inner join PartMaterialRequestDetail_PO pmrd on pmrid.RequestDetailID = pmrd.RequestDetailID
	where pmrd.RequestID = @RequestId

	delete From PartMaterialRequestDetail_PO where RequestID = @RequestId

	delete From PartMaterialRequestHeader_PO where RequestID = @RequestId
end
GO
