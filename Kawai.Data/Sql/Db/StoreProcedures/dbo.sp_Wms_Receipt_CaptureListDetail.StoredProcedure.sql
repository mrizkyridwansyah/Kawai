SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_Receipt_CaptureListDetail]
	@ReceiptId bigint
as

select * From PartReceiptDetail where ReceiptId = @ReceiptId
GO
