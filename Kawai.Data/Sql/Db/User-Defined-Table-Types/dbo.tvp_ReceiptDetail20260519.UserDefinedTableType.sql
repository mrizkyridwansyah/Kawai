CREATE TYPE [tvp_ReceiptDetail20260519] AS TABLE(
	[PONumber] [varchar](50) NULL,
	[ItemCode] [varchar](25) NULL,
	[UnitClsCode] [varchar](25) NULL,
	[ExpectedQty] [numeric](18, 9) NULL,
	[TotalPacking] [numeric](18, 9) NULL,
	[ReceiptQty] [numeric](18, 9) NULL,
	[NoSeri] [int] NULL
)
GO
