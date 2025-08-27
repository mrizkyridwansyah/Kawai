SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Requirement_Master](
	[ChildRequirement_Year] [numeric](4, 0) NOT NULL,
	[ChildRequirement_Month] [numeric](2, 0) NOT NULL,
	[ChildItem_Code] [char](25) NOT NULL,
	[ChildRequirement_Qty] [numeric](18, 5) NULL,
	[ChildRequirementResult_Qty] [numeric](18, 5) NULL,
	[ChildUnit_Cls] [char](2) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Requirement_Master] PRIMARY KEY CLUSTERED 
(
	[ChildRequirement_Year] ASC,
	[ChildRequirement_Month] ASC,
	[ChildItem_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
