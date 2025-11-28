/****** Object:  StoredProcedure [dbo].[sp_Wms_IQCResult_GetDetail]    Script Date: 11/28/2025 10:55:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [dbo].[sp_Wms_IQCResult_GetDetail]
	@InspectionId bigint
as
begin
	select 
		iqch.InspectionID InspectionId, 
		prh.DNNumber,
		iqch.ItemCode, 
		iqch.ItemName,
		Qty = iqch.TotalQtySample, 
		QtyNG = isnull(iqch.TotalQtyNG, 0), 
		iqch.InspectionResult, 
		iqch.Remarks,
		att.AttachmentID,
		att.[FileName] AttachmentFileName
	from IQC_Inspection_Header iqch
	inner join PartReceiptHeader prh on iqch.ReceiptNo = prh.ReceiptNo
	left join IQC_Attachment att on iqch.InspectionID = att.InspectionID
	where iqch.InspectionID = @InspectionId

end
GO
