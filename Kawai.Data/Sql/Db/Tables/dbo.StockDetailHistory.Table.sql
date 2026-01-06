SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [StockDetailHistory](
	[Period] [varchar](6) NOT NULL,
	[RefNo] [varchar](50) NOT NULL,
	[WarehouseCode] [varchar](25) NOT NULL,
	[AreaCode] [varchar](25) NOT NULL,
	[AddressCode] [varchar](25) NOT NULL,
	[BarcodeNo] [varchar](50) NOT NULL,
	[ItemCode] [varchar](25) NOT NULL,
	[LotNo] [varchar](100) NOT NULL,
	[SublotNo] [int] NULL,
	[Qty] [numeric](18, 9) NULL,
	[InventoryQty] [numeric](18, 9) NULL,
	[ExpiredDate] [date] NULL,
	[ProductionDate] [date] NULL,
	[ReceiptDate] [date] NULL,
	[Supplier] [varchar](150) NULL,
	[PrintCls] [bit] NULL,
	[DisposalCls] [bit] NULL,
	[StatusReceipt] [varchar](10) NULL,
	[RegisterDate] [datetime] NULL,
	[RegisterUser] [varchar](15) NULL,
	[Lastupdate] [datetime] NULL,
	[LastUser] [varchar](15) NULL,
	[ClosingDate] [datetime] NULL,
	[ClosingUser] [varchar](15) NULL,
 CONSTRAINT [PK_Stock_Detail_History] PRIMARY KEY CLUSTERED 
(
	[Period] ASC,
	[RefNo] ASC,
	[WarehouseCode] ASC,
	[AreaCode] ASC,
	[AddressCode] ASC,
	[BarcodeNo] ASC,
	[ItemCode] ASC,
	[LotNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [StockDetailHistory]  WITH CHECK ADD  CONSTRAINT [CK__StockDetailHistory] CHECK  (([StatusReceipt]='HOLD' OR [StatusReceipt]='OK' OR [StatusReceipt]='NG'))
GO
ALTER TABLE [StockDetailHistory] CHECK CONSTRAINT [CK__StockDetailHistory]
GO
