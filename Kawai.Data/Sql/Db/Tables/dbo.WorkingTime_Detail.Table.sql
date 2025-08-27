SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [WorkingTime_Detail](
	[ProductionSeq_No] [numeric](18, 0) NOT NULL,
	[WorkingLossTime_Cls] [char](2) NOT NULL,
	[Loss_Time] [numeric](5, 0) NULL,
	[Remarks] [char](35) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_WorkingTime_Detail] PRIMARY KEY CLUSTERED 
(
	[ProductionSeq_No] ASC,
	[WorkingLossTime_Cls] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [WorkingTime_Detail]  WITH NOCHECK ADD  CONSTRAINT [FK_WorkingTime_Detail_WorkingTime_Master] FOREIGN KEY([ProductionSeq_No])
REFERENCES [WorkingTime_Master] ([ProductionSeq_No])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [WorkingTime_Detail] CHECK CONSTRAINT [FK_WorkingTime_Detail_WorkingTime_Master]
GO
ALTER TABLE [WorkingTime_Detail]  WITH NOCHECK ADD  CONSTRAINT [FK_WorkingTime_Detail_WorkingTimeLoss_Cls] FOREIGN KEY([WorkingLossTime_Cls])
REFERENCES [WorkingLossTime_Cls] ([WorkingLossTime_Cls])
GO
ALTER TABLE [WorkingTime_Detail] CHECK CONSTRAINT [FK_WorkingTime_Detail_WorkingTimeLoss_Cls]
GO
