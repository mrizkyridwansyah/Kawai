SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Price_Master](
	[Price_Cls] [char](2) NOT NULL,
	[Item_Code] [char](25) NOT NULL,
	[Trade_Code] [char](15) NOT NULL,
	[Priority_Cls] [char](1) NOT NULL,
	[Currency_Code] [char](2) NULL,
	[Price] [numeric](18, 5) NULL,
	[Unit_Cls] [char](2) NULL,
	[Start_Date] [char](8) NOT NULL,
	[End_Date] [char](8) NULL,
	[Reason_Cls] [char](2) NULL,
	[Remarks] [char](250) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Price_Master] PRIMARY KEY CLUSTERED 
(
	[Price_Cls] ASC,
	[Item_Code] ASC,
	[Trade_Code] ASC,
	[Priority_Cls] ASC,
	[Start_Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
