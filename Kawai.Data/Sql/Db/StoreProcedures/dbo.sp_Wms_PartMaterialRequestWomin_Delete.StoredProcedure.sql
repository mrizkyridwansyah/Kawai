SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [sp_Wms_PartMaterialRequestWomin_Delete]
	@RequestId bigint,
	@UserId varchar(25)
as
begin
	delete pmrid 
	From PartMaterialRequestItemDetail pmrid
	inner join PartMaterialRequestDetail pmrd on pmrid.RequestDetailID = pmrd.RequestDetailID
	where pmrd.RequestID = @RequestId

	delete From PartMaterialRequestDetail where RequestID = @RequestId

	delete From PartMaterialRequestHeader where RequestID = @RequestId
end
GO
