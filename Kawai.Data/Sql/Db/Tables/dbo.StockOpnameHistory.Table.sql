SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [StockOpnameHistory](
	[Period] [varchar](50) NOT NULL,
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
	[LastUser] [varchar](25) NULL
) ON [PRIMARY]
GO
