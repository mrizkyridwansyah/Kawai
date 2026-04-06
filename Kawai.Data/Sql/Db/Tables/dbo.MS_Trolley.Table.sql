SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [MS_Trolley](
	[SeNo] [bigint] IDENTITY(1,1) NOT NULL,
	[TrolleyCode] [varchar](20) NOT NULL,
	[Description] [varchar](150) NULL,
	[Trolley_Cls] [varchar](2) NULL,
	[IsActive] [bit] NOT NULL,
	[RegisterDate] [datetime] NOT NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL,
	[LastPosition] [varchar](100) NULL
) ON [PRIMARY]
GO
