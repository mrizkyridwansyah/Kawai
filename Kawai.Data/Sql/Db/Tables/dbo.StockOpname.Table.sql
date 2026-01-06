SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [StockOpname](
	[BarcodeNo] [varchar](50) NOT NULL,
	[LotNo] [varchar](100) NOT NULL,
	[ItemCode] [varchar](25) NOT NULL,
	[RefNo] [varchar](50) NOT NULL,
	[WarehouseCode] [varchar](25) NOT NULL,
	[AreaCode] [varchar](25) NOT NULL,
	[AddressCode] [varchar](25) NOT NULL,
	[InventoryQty] [numeric](18, 9) NOT NULL,
	[RegisterDate] [datetime] NOT NULL,
	[RegisterUser] [varchar](25) NOT NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL,
 CONSTRAINT [PK_StockOpname] PRIMARY KEY CLUSTERED 
(
	[BarcodeNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
