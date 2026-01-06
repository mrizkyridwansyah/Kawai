SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [StockHeaderHistory](
	[Period] [varchar](6) NOT NULL,
	[RefNo] [varchar](50) NOT NULL,
	[WarehouseCode] [varchar](25) NOT NULL,
	[AreaCode] [varchar](25) NOT NULL,
	[ItemCode] [varchar](25) NOT NULL,
	[LotNo] [varchar](50) NOT NULL,
	[PreMonth] [numeric](18, 9) NULL,
	[Receipt] [numeric](18, 9) NULL,
	[Supply] [numeric](18, 9) NULL,
	[LossReject] [numeric](18, 9) NULL,
	[Current] [numeric](18, 9) NULL,
	[Inventory] [numeric](18, 9) NULL,
	[Reason] [varchar](255) NULL,
	[Adjustment] [numeric](18, 9) NULL,
	[RegisterUpdate] [datetime] NULL,
	[RegisterUser] [varchar](15) NULL,
 CONSTRAINT [PK_StockMaster_History] PRIMARY KEY CLUSTERED 
(
	[Period] ASC,
	[RefNo] ASC,
	[WarehouseCode] ASC,
	[AreaCode] ASC,
	[ItemCode] ASC,
	[LotNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
