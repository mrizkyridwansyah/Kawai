SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ProductionCalculate_DetailPlan](
	[Cust_Code] [char](15) NOT NULL,
	[PO_No] [char](35) NOT NULL,
	[Seq_No] [numeric](18, 0) NOT NULL,
	[Plan_SeqNo] [numeric](18, 0) NOT NULL,
	[PlanItem_Code] [char](25) NOT NULL,
	[Plan_Cls] [char](1) NULL,
	[Plan_Date] [datetime] NULL,
	[Plan_Qty] [numeric](18, 5) NULL,
	[Generate_Cls] [char](1) NULL,
	[DailySeq_No] [numeric](18, 0) NOT NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_ProductionCalculate_DetailPlan] PRIMARY KEY CLUSTERED 
(
	[Cust_Code] ASC,
	[PO_No] ASC,
	[Seq_No] ASC,
	[Plan_SeqNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
