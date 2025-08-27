SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [UPLOAD_PRICE](
	[Inventory_Year] [numeric](4, 0) NOT NULL,
	[Inventory_Month] [numeric](2, 0) NOT NULL,
	[Item_Code] [char](25) NOT NULL,
	[Current_Stock] [numeric](18, 5) NULL,
	[Current_Price] [numeric](18, 5) NULL
) ON [PRIMARY]
GO
