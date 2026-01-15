DROP TYPE IF EXISTS [tvp_StockDetail]
CREATE TYPE [tvp_StockDetail] AS TABLE(
	[WarehouseCode] [varchar](50) NULL,
	[AreaCode] [varchar](25) NULL,
	[AddressCode] [varchar](25) NULL,
	[ItemCode] [varchar](25) NULL,
	[LotNo] [varchar](100) NULL,
	[InventoryQty] [numeric](18, 9) NULL
)
GO