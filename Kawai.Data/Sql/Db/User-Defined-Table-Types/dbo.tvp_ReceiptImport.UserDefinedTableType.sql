CREATE TYPE [dbo].[tvp_ReceiptImport] AS TABLE(
	[PONumber] [varchar](25) NOT NULL,
	[ItemCode] [varchar](25) NOT NULL,
	[ReceiptQty] [int] NULL,
	[RowNumber] [int] NULL,
	[Errors] [varchar](max) NULL
)
GO