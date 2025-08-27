SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Requirement](
	[ParentItem_Code] [char](25) NOT NULL,
	[Lot_No] [char](7) NOT NULL,
	[Factory_Code] [char](15) NULL,
	[Line_Code] [char](15) NULL,
	[Production_Date] [datetime] NOT NULL,
	[Qty] [numeric](18, 5) NULL,
	[Off_Qty] [numeric](18, 5) NULL,
	[ChildItem_Code] [char](25) NOT NULL,
	[ChildRequirement_Qty] [numeric](18, 5) NULL,
	[OffChildRequirement_Qty] [numeric](18, 5) NULL,
	[ChildRequirementResult_Qty] [numeric](18, 5) NULL,
	[ChildUnit_Cls] [char](2) NULL,
	[ChildRequirement_Date] [datetime] NULL,
	[Complete_Cls] [char](1) NULL,
	[TempQty] [numeric](18, 5) NULL,
	[NonMRP_Cls] [char](1) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Requirement] PRIMARY KEY CLUSTERED 
(
	[ParentItem_Code] ASC,
	[Lot_No] ASC,
	[Production_Date] ASC,
	[ChildItem_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
