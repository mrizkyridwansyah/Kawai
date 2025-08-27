SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TempNormal_Price](
	[Item_code] [char](25) NULL,
	[item_name] [char](100) NULL,
	[Trade_Code] [char](15) NULL,
	[Trade_Abbr] [char](20) NULL,
	[Price] [numeric](18, 5) NULL
) ON [PRIMARY]
GO
