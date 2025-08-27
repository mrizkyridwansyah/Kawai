SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TempValuation_PriceCalc](
	[parent_itemcode] [char](25) NULL,
	[item_name] [char](100) NULL,
	[Valuation_Price] [numeric](18, 5) NULL,
	[last_update] [datetime] NULL
) ON [PRIMARY]
GO
