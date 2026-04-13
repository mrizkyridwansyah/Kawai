SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   procedure [sp_Wms_Receipt_CaptureHeader]
	@Id bigint
as

select Id, ReceiptNo, ReceiptDate, SupplierCode, DNNumber, DNDate, BCNumber, BCType, BCDate, VehicleNo, StatusReceipt, IsManual, Transport, CompanyCode [FactoryCode] , RegisterNo
From PartReceiptHeader where Id = @Id
GO
