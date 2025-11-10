SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create procedure [sp_Wms_Receipt_CaptureDataBarcode]
	@Id bigint
as
	select * From PartReceiptDetailBarcode where Id = @Id
GO
