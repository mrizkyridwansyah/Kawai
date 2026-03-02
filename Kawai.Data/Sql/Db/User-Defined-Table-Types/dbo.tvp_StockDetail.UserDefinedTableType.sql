DROP TYPE IF EXISTS [tvp_StockDetail]
CREATE TYPE [tvp_StockDetail] AS TABLE(
	[RefNo] [varchar](50) NULL,
	[WarehouseCode] [varchar](50) NULL,
	[AreaCode] [varchar](25) NULL,
	[AddressCode] [varchar](25) NULL,
	[BarcodeNo] [varchar](50) NULL,
	[ItemCode] [varchar](25) NULL,
	[LotNo] [varchar](50) NULL,
	[InventoryQty] [numeric](18, 9) NULL
)
GO