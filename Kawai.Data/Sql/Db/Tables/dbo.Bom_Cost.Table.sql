SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Bom_Cost](
	[Parent_ItemCode] [char](25) NOT NULL,
	[Period] [datetime] NOT NULL,
	[Supplier_Code] [char](15) NULL,
	[Item_Code] [char](25) NULL,
	[HS_Code] [char](15) NULL,
	[Item_Name] [char](75) NULL,
	[Qty] [numeric](18, 5) NULL,
	[Unit] [varchar](25) NULL,
	[Currency] [char](5) NULL,
	[Price] [numeric](18, 5) NULL,
	[Value] [numeric](18, 5) NULL,
	[BC_Type] [varchar](15) NULL,
	[BC40_No] [char](30) NULL,
	[BC40_Date] [datetime] NULL,
	[Region] [char](50) NULL,
	[Origin_Country] [char](50) NULL,
	[Percentage] [numeric](18, 0) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[Trade_Name] [char](70) NULL
) ON [PRIMARY]
GO
