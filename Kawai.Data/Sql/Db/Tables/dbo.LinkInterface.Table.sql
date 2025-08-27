SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [LinkInterface](
	[JournalType] [varchar](2) NULL,
	[TransCode] [varchar](5) NULL,
	[Trade_Code] [char](15) NULL,
	[Curr_Code] [char](2) NULL,
	[Posting_Key] [varchar](2) NULL,
	[Account_No] [varchar](10) NULL,
	[Tax_Code] [char](2) NULL,
	[Cost_Center] [char](10) NULL,
	[Profit_Center] [char](10) NULL
) ON [PRIMARY]
GO
