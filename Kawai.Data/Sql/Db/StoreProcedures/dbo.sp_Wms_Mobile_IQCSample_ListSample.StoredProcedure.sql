SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Mobile_IQCSample_ListSample]
	@ReceiptId bigint
as
begin
	SELECT 
		dtl.ItemCode, mi.Item_Name ItemName, iqc.TotalQtySample
	FROM 
	(
		select a.ItemCode, b.ReceiptNo, sum(a.ReceiptQty) ReceiptQty From PartReceiptDetail a
		inner join PartReceiptHeader b on a.ReceiptId = b.Id
		where ReceiptId = @ReceiptId
		group by a.ItemCode, b.ReceiptNo
	) dtl
	left join Item_Master mi on dtl.ItemCode = mi.Item_Code
	left join 
	(
		SELECT * fROM IQC_Inspection_Header WHERE Soruce = 'Incoming Material'
	) iqc on dtl.ReceiptNo = iqc.ReceiptNo and dtl.ItemCode = iqc.ItemCode
end

GO
