SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Trolley_Cls](
	[Trolley_Cls] [varchar](2) NOT NULL,
	[Description] [varchar](100) NULL,
	[Qty] [int] NULL,
	[RegisterDate] [datetime] NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL
) ON [PRIMARY]
GO
