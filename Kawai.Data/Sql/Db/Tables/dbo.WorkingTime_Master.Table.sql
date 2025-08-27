SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [WorkingTime_Master](
	[ProductionSeq_No] [numeric](18, 0) NOT NULL,
	[Working_Time] [numeric](5, 0) NULL,
	[TotalLoss_Time] [numeric](5, 0) NULL,
	[TotalWorking_Time] [numeric](5, 0) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_WorkingTime_Master] PRIMARY KEY CLUSTERED 
(
	[ProductionSeq_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
