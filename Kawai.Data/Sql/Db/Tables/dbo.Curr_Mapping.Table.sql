SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Curr_Mapping](
	[EZRCurr] [varchar](2) NULL,
	[SAP_Curr] [varchar](3) NULL,
	[Update_User] [varchar](15) NULL,
	[LastUpdate] [datetime] NULL
) ON [PRIMARY]
GO
