SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [FG_Price](
	[parent_itemcode] [char](25) NULL,
	[item_name] [char](100) NULL,
	[Material_price] [numeric](18, 5) NULL,
	[last_update] [datetime] NULL
) ON [PRIMARY]
GO
