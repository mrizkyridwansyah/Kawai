SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [StockHeader](
	[RefNo] [varchar](50) NOT NULL,
	[WarehouseCode] [varchar](25) NOT NULL,
	[AreaCode] [varchar](25) NOT NULL,
	[ItemCode] [varchar](25) NOT NULL,
	[LotNo] [varchar](50) NOT NULL,
	[LMPreMonth] [numeric](18, 9) NULL,
	[LMReceipt] [numeric](18, 9) NULL,
	[LMSupply] [numeric](18, 9) NULL,
	[LMLossReject] [numeric](18, 9) NULL,
	[LMCurrent] [numeric](18, 9) NULL,
	[LMInventory] [numeric](18, 9) NULL,
	[TMPreMonth] [numeric](18, 9) NULL,
	[TMReceipt] [numeric](18, 9) NULL,
	[TMSupply] [numeric](18, 9) NULL,
	[TMLossReject] [numeric](18, 9) NULL,
	[TMCurrent] [numeric](18, 9) NULL,
	[TMInventory] [numeric](18, 9) NULL,
	[NMPreMonth] [numeric](18, 9) NULL,
	[NMReceipt] [numeric](18, 9) NULL,
	[NMSupply] [numeric](18, 9) NULL,
	[NMLossReject] [numeric](18, 9) NULL,
	[NMCurrent] [numeric](18, 9) NULL,
	[NMInventory] [numeric](18, 9) NULL,
	[LMReason] [varchar](255) NULL,
	[TMReason] [varchar](255) NULL,
	[NMReason] [varchar](255) NULL,
	[Adjustment] [numeric](18, 9) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [char](15) NULL,
	[RegisterDate] [datetime] NULL,
 CONSTRAINT [PK_StockMaster] PRIMARY KEY CLUSTERED 
(
	[RefNo] ASC,
	[WarehouseCode] ASC,
	[AreaCode] ASC,
	[ItemCode] ASC,
	[LotNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
