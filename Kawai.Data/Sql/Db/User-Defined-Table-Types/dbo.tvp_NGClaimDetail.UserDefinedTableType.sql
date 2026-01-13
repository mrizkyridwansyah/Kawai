CREATE TYPE [tvp_NGClaimDetail] AS TABLE(
	[PONumber] [varchar](50) NULL,
	[ReceiptNumber] [varchar](50) NULL,
	[ItemCode] [varchar](25) NULL,
	[Qty] [numeric](18, 9) NULL,
	[NGCode] [varchar](25) NULL
)
GO
