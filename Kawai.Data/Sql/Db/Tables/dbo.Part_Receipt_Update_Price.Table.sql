SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Part_Receipt_Update_Price](
	[Po_No] [char](25) NULL,
	[Receiptseq_no] [numeric](18, 0) NULL,
	[Supplier_Code] [char](15) NULL,
	[Item_Code] [char](25) NULL,
	[Price_Old] [numeric](18, 5) NULL,
	[Currency_Code_Old] [char](2) NULL,
	[Price_New] [numeric](18, 5) NULL,
	[Currency_Code_New] [char](2) NULL,
	[Process_Date] [datetime] NULL,
	[User_ID] [char](15) NULL
) ON [PRIMARY]
GO
