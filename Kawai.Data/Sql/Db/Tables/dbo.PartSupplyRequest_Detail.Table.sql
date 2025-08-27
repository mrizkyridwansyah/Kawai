SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PartSupplyRequest_Detail](
	[SupplyRec_No] [char](25) NOT NULL,
	[Seq_No] [numeric](18, 0) NOT NULL,
	[ChildItem_Code] [char](25) NULL,
	[ChildLot_No] [char](7) NULL,
	[ChildRequirement_Qty] [numeric](18, 5) NOT NULL,
	[ChildUnit_Cls] [char](2) NULL,
	[ParentItem_Code] [char](25) NULL,
	[Remarks] [char](35) NULL,
	[DailySeq_No] [numeric](18, 0) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[ReplacementItem_Code] [char](25) NULL,
 CONSTRAINT [PK_PartSupplyRequest_Detail] PRIMARY KEY CLUSTERED 
(
	[SupplyRec_No] ASC,
	[Seq_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [PartSupplyRequest_Detail]  WITH CHECK ADD  CONSTRAINT [FK_PartSupplyRequest_Detail_PartSupplyRequest_Master] FOREIGN KEY([SupplyRec_No])
REFERENCES [PartSupplyRequest_Master] ([SupplyRec_No])
ON DELETE CASCADE
GO
ALTER TABLE [PartSupplyRequest_Detail] CHECK CONSTRAINT [FK_PartSupplyRequest_Detail_PartSupplyRequest_Master]
GO
