SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BarcodeNGDetail](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ReceiptId] [bigint] NOT NULL,
	[DNNumber] [varchar](100) NOT NULL,
	[SupplierCode] [varchar](25) NOT NULL,
	[ItemCode] [varchar](25) NOT NULL,
	[PONumber] [varchar](100) NULL,
	[BarcodeOriginal] [varchar](50) NOT NULL,
	[BarcodeNew] [varchar](50) NOT NULL,
	[QtyNG] [numeric](18, 9) NULL
) ON [PRIMARY]
GO
