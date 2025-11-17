SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_Receipt_CaptureHeader]
	@Id bigint
as

select Id, ReceiptNo, ReceiptDate, SupplierCode, DNNumber, DNDate, BCNumber, BCType, BCDate, VehicleNo, StatusReceipt, IsManual, Transport  
From PartReceiptHeader where Id = @Id
GO
