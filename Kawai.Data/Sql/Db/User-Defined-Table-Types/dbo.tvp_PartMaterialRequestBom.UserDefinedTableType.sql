CREATE TYPE [tvp_PartMaterialRequestBom] AS TABLE(
	[RequestId] [bigint] NULL,
	[PONumber] [varchar](50) NULL,
	[PODate] [date] NULL,
	[ItemCode] [varchar](25) NULL,
	[RequestSetQty] [numeric](18, 9) NULL
)
GO
