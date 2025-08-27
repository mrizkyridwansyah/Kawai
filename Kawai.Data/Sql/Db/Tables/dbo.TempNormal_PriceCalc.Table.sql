SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TempNormal_PriceCalc](
	[parent_itemcode] [char](25) NULL,
	[item_name] [char](100) NULL,
	[Normal_PriceCalc] [numeric](18, 5) NULL,
	[last_update] [datetime] NULL
) ON [PRIMARY]
GO
