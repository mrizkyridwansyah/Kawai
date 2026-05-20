CREATE TABLE [dbo].[PartReceiptDetailBarcodeDeleted](
	[ReceiptId] [bigint] NOT NULL,
	[BarcodeNo] [varchar](50) NOT NULL,
	[Qty] [numeric](18, 9) NOT NULL
) ON [PRIMARY]
GO
