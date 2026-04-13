SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create   procedure [sp_Wms_PartMaterialRequestWomin_Delete]
	@RequestId bigint,
	@UserId varchar(25)
as
begin
	if exists 
	(
		select 1 From PartMaterialRequestItemDetailScan a
		inner join PartMaterialRequestItemDetail b on a.IDSeq = b.IDSeq
		inner join PartMaterialRequestDetail c on b.RequestDetailID = c.RequestDetailID
		where c.RequestID = @RequestId
	)
	begin
		raiserror('Data ini sudah memiliki transaksi supply scan!', 16, 1)
		return
	end

	delete pmrid 
	From PartMaterialRequestItemDetail pmrid
	inner join PartMaterialRequestDetail pmrd on pmrid.RequestDetailID = pmrd.RequestDetailID
	where pmrd.RequestID = @RequestId

	delete From PartMaterialRequestDetail where RequestID = @RequestId

	delete From PartMaterialRequestHeader where RequestID = @RequestId
end
GO
