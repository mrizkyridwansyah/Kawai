SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Actual_Production](
	[Item_Code] [char](25) NOT NULL,
	[Lot_No] [char](7) NOT NULL,
	[Actual_Date] [char](10) NOT NULL,
	[Qty] [numeric](18, 5) NULL,
	[Actual_Cls] [char](1) NOT NULL,
	[Off_Qty] [numeric](18, 5) NULL,
	[Complete_Cls] [char](1) NULL,
 CONSTRAINT [PK_Actual_Production] PRIMARY KEY CLUSTERED 
(
	[Item_Code] ASC,
	[Lot_No] ASC,
	[Actual_Date] ASC,
	[Actual_Cls] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
