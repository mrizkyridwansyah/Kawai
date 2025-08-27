SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Psi](
	[Year] [char](4) NULL,
	[Month] [char](2) NULL,
	[DP_Kawai] [numeric](18, 5) NULL,
	[DP_KE] [numeric](18, 5) NULL,
	[DP_China] [numeric](18, 5) NULL,
	[DP_Import] [numeric](18, 5) NULL,
	[DP_HI] [numeric](18, 5) NULL,
	[DP_MPFI] [numeric](18, 5) NULL,
	[DP_KAI] [numeric](18, 5) NULL,
	[DP_Local] [numeric](18, 5) NULL,
	[Key_Kawai] [numeric](18, 5) NULL,
	[Key_China] [numeric](18, 5) NULL,
	[Key_Import] [numeric](18, 5) NULL,
	[Key_HI] [numeric](18, 5) NULL,
	[Key_Local] [numeric](18, 5) NULL,
	[Key_KAI] [numeric](18, 5) NULL,
	[Key_KMK] [numeric](18, 5) NULL,
	[KU] [char](10) NULL,
	[Pedal] [char](10) NULL,
	[Material_Price] [numeric](18, 5) NULL,
	[Assy_Cost] [numeric](18, 5) NULL,
	[FG_Price] [numeric](18, 5) NULL,
	[FOB] [numeric](18, 5) NULL,
	[FG_Price / FOB] [numeric](18, 5) NULL,
	[Output/jam] [numeric](18, 5) NULL,
	[Works_hour] [numeric](18, 5) NULL,
	[Expense_Ratio] [numeric](18, 5) NULL,
	[Parentitem_code] [char](15) NULL,
	[Description] [char](75) NULL,
	[PI] [numeric](18, 5) NULL,
	[premonth] [numeric](18, 5) NULL,
	[Production] [numeric](18, 5) NULL,
	[Export] [numeric](18, 5) NULL,
	[End_stock] [numeric](18, 5) NULL,
	[Inventory_Amount] [numeric](18, 5) NULL,
	[NumberOfWorkers] [int] NULL
) ON [PRIMARY]
GO
