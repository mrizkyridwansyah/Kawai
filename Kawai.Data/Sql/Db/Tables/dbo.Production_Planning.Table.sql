SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Production_Planning](
	[Prod_Year] [numeric](4, 0) NOT NULL,
	[Prod_Month] [numeric](2, 0) NOT NULL,
	[Item_Code] [char](25) NOT NULL,
	[Lot_No] [char](7) NULL,
	[Qty] [numeric](18, 5) NULL,
	[Unit_Cls] [char](2) NULL,
	[Production_Date] [datetime] NULL,
	[Complete_cls] [char](1) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Production_Planning] PRIMARY KEY CLUSTERED 
(
	[Item_Code] ASC,
	[Prod_Year] ASC,
	[Prod_Month] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
