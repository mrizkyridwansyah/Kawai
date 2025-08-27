SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Delivery_Return](
	[Cust_Code] [char](15) NULL,
	[Do_No] [char](25) NOT NULL,
	[Po_No] [char](35) NOT NULL,
	[Seq_No] [numeric](18, 0) NOT NULL,
	[DoSeq_No] [numeric](18, 0) NOT NULL,
	[Item_Code] [char](25) NOT NULL,
	[ReturnSeq_No] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
	[Return_Date] [datetime] NULL,
	[Reference] [char](25) NULL,
	[Return_Qty] [numeric](18, 5) NULL,
	[Unit_Cls] [char](2) NULL,
	[Lot_No] [char](15) NULL,
	[Curr_Code] [char](2) NULL,
	[Price] [numeric](18, 5) NULL,
	[Service] [numeric](18, 5) NULL,
	[Amount] [numeric](22, 5) NULL,
	[WH_Code] [char](15) NULL,
	[Return_Cls] [char](2) NULL,
	[Remarks] [char](50) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Delivery_Return] PRIMARY KEY CLUSTERED 
(
	[Do_No] ASC,
	[Po_No] ASC,
	[Seq_No] ASC,
	[DoSeq_No] ASC,
	[Item_Code] ASC,
	[ReturnSeq_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
