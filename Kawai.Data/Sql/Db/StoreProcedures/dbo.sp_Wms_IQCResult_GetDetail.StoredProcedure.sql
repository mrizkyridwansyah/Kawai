
create   procedure [dbo].[sp_Wms_IQCResult_GetDetail]
	@InspectionId bigint
as
begin

	select 
		iqch.InspectionID InspectionId, 
		prh.DNNumber,
		iqch.ItemCode, 
		iqch.ItemName,
		Qty = iqch.TotalQtySample, 
		QtyReceipt = pdtl.ReceiptQty,
		QtyNG = isnull(iqch.TotalQtyNG, 0), 
		iqch.InspectionResult, 
		iqch.Remarks,
		iqch.RemarksSA,
		att.AttachmentID,
		att.[FileName] AttachmentFileName
	from IQC_Inspection_Header iqch
	inner join PartReceiptHeader prh on iqch.ReceiptNo = prh.ReceiptNo
	inner join 
	(
		select ReceiptId, ItemCode, sum(ReceiptQty) ReceiptQty From PartReceiptDetail group by ReceiptId, ItemCode 
	) pdtl on pdtl.ReceiptId = prh.Id and pdtl.ItemCode = iqch.ItemCode
	left join IQC_Attachment att on iqch.InspectionID = att.InspectionID
	where iqch.InspectionID = @InspectionId

end
