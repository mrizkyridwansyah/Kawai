SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [InventoryPrice_History](
	[Inventory_Year] [numeric](4, 0) NOT NULL,
	[Inventory_Month] [numeric](2, 0) NOT NULL,
	[Item_Code] [char](25) NOT NULL,
	[Duty_Status] [char](1) NOT NULL,
	[Premonth_Stock] [numeric](18, 5) NULL,
	[Premonth_Price] [numeric](18, 5) NULL,
	[Incoming_Stock] [numeric](18, 5) NULL,
	[Incoming_Price] [numeric](18, 5) NULL,
	[IncomingOther_Stock] [numeric](18, 5) NULL,
	[IncomingOther_Price] [numeric](18, 5) NULL,
	[Outgoing_Stock] [numeric](18, 5) NULL,
	[Outgoing_Price] [numeric](18, 5) NULL,
	[OutgoingOther_Stock] [numeric](18, 5) NULL,
	[OutgoingOther_Price] [numeric](18, 5) NULL,
	[Current_Stock] [numeric](18, 5) NULL,
	[Current_Price] [numeric](18, 5) NULL,
	[Inventory_Price] [numeric](18, 5) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_InventoryPrice_History_1] PRIMARY KEY CLUSTERED 
(
	[Inventory_Year] ASC,
	[Inventory_Month] ASC,
	[Item_Code] ASC,
	[Duty_Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
