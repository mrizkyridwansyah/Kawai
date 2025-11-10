SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [sp_Wms_Receipt_Capture]
	@Id bigint
as

select 
	Id, ReceiptNo, ReceiptDate, SupplierCode, DNNumber, DNDate, BCNumber, BCType, BCDate, VehicleNo, StatusReceipt, IsManual, Transport 
From PartReceiptHeader where Id = @Id

select * From PartReceiptDetail where ReceiptId = @Id

select * From PartReceiptDetailBarcode where ReceiptId = @Id
GO
