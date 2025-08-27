SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Price_Master_Contract](
	[Price_Cls] [varchar](2) NOT NULL,
	[Item_Code] [varchar](25) NOT NULL,
	[Trade_Code] [varchar](15) NOT NULL,
	[Priority_Cls] [varchar](1) NOT NULL,
	[Currency_Code] [varchar](2) NULL,
	[Price] [numeric](18, 5) NULL,
	[Unit_Cls] [varchar](2) NULL,
	[Start_Date] [varchar](8) NOT NULL,
	[End_Date] [varchar](8) NULL,
	[Reason_Cls] [varchar](2) NULL,
	[Qty_Contract] [numeric](18, 2) NULL,
	[Status_Closing] [varchar](2) NULL,
	[Remarks] [varchar](250) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [varchar](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Price_Master_Contract] PRIMARY KEY CLUSTERED 
(
	[Price_Cls] ASC,
	[Item_Code] ASC,
	[Trade_Code] ASC,
	[Priority_Cls] ASC,
	[Start_Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
