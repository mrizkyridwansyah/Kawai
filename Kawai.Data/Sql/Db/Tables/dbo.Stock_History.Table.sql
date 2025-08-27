SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Stock_History](
	[Stock_Year] [char](4) NOT NULL,
	[Stock_Month] [char](2) NOT NULL,
	[Warehouse_Code] [char](15) NOT NULL,
	[Item_Code] [char](25) NOT NULL,
	[PreMonth] [numeric](18, 5) NULL,
	[Inventory] [numeric](18, 5) NULL,
	[Receipt] [numeric](18, 5) NULL,
	[Supply] [numeric](18, 5) NULL,
	[LossReject] [numeric](18, 5) NULL,
	[Current] [numeric](18, 5) NULL,
	[Reason] [varchar](255) NULL,
 CONSTRAINT [PK_Stock_History] PRIMARY KEY CLUSTERED 
(
	[Stock_Year] ASC,
	[Stock_Month] ASC,
	[Warehouse_Code] ASC,
	[Item_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
