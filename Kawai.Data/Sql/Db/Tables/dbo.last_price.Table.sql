SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [last_price](
	[item_code] [char](25) NOT NULL,
	[price] [numeric](32, 13) NULL,
	[trade_abbr] [char](20) NULL
) ON [PRIMARY]
GO
