/****** Object:  StoredProcedure [dbo].[sp_Wms_IQCResult_Capture]    Script Date: 11/28/2025 10:55:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [dbo].[sp_Wms_IQCResult_Capture]
	@Id bigint 
as
begin
	declare @attachmentId bigint, @attachmentName varchar(max)
	select 
		top 1 @attachmentId = AttachmentID, @attachmentName = [FileName] 
	from IQC_Attachment 
	where InspectionID = @Id 
	order by RegisterDate desc

	select *, @attachmentId AttachmentId, @attachmentName AttachmentName
	From IQC_Inspection_Header iqch
	where InspectionID = @Id

	select * From IQC_SamplingBarcodeDetail where InspectionID = @Id
end
GO
