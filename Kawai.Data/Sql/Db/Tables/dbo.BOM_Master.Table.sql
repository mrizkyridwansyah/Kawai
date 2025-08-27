SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BOM_Master](
	[Parent_ItemCode] [char](25) NOT NULL,
	[Description] [char](75) NULL,
	[Item_Code] [char](25) NOT NULL,
	[Item_Name] [char](75) NULL,
	[Qty] [numeric](18, 5) NULL,
	[R_Qty] [numeric](18, 5) NULL,
	[Unit_Cls] [char](2) NULL,
	[Currency_Code] [char](2) NULL,
	[Accounting_Code] [char](7) NULL,
	[Start_Date] [char](8) NOT NULL,
	[End_date] [char](8) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[Doc_No] [char](50) NULL,
	[Revision_No] [char](50) NULL,
 CONSTRAINT [PK_BOM_Master] PRIMARY KEY CLUSTERED 
(
	[Parent_ItemCode] ASC,
	[Item_Code] ASC,
	[Start_Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
