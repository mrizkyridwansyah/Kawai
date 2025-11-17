CREATE TYPE [tvp_ReceiptDetailVerify] AS TABLE(
	[ReceiptDetailBarcodeId] [bigint] NULL,
	[BarcodeNo] [varchar](50) NULL,
	[Qty] [numeric](18, 9) NULL,
	[QtyVerify] [numeric](18, 9) NULL
)
GO
