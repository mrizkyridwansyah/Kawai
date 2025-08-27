SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Stock_Master](
	[Warehouse_Code] [char](15) NOT NULL,
	[Item_Code] [char](25) NOT NULL,
	[LM_PreMonth] [numeric](18, 5) NULL,
	[LM_Receipt] [numeric](18, 5) NULL,
	[LM_Supply] [numeric](18, 5) NULL,
	[LM_LossReject] [numeric](18, 5) NULL,
	[LM_Current] [numeric](18, 5) NULL,
	[LM_Inventory] [numeric](18, 5) NULL,
	[LM_Reason] [char](50) NULL,
	[TM_PreMonth] [numeric](18, 5) NULL,
	[TM_Receipt] [numeric](18, 5) NULL,
	[TM_Supply] [numeric](18, 5) NULL,
	[TM_LossReject] [numeric](18, 5) NULL,
	[TM_Current] [numeric](18, 5) NULL,
	[TM_Inventory] [numeric](18, 5) NULL,
	[TM_Reason] [char](50) NULL,
	[NM_PreMonth] [numeric](18, 5) NULL,
	[NM_Receipt] [numeric](18, 5) NULL,
	[NM_Supply] [numeric](18, 5) NULL,
	[NM_LossReject] [numeric](18, 5) NULL,
	[NM_Current] [numeric](18, 5) NULL,
	[NM_Inventory] [numeric](18, 5) NULL,
	[NM_Reason] [char](50) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Stock_Master] PRIMARY KEY CLUSTERED 
(
	[Warehouse_Code] ASC,
	[Item_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
