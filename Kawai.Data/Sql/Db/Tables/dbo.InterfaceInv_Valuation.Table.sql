SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [InterfaceInv_Valuation](
	[Period] [char](8) NULL,
	[Account_No] [varchar](15) NULL,
	[Amount] [numeric](18, 2) NULL,
	[Posting_Key] [varchar](2) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [varchar](15) NULL,
	[Register_Date] [datetime] NULL
) ON [PRIMARY]
GO
