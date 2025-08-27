SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [sp_Wms_Receipt_CaptureHeader]
	@Id bigint
as

select * From PartReceiptHeader where Id = @Id
GO
