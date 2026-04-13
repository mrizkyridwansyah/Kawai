SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [MS_StopPoint](
	[StopPointCode] [varchar](25) NOT NULL,
	[Description] [nvarchar](200) NULL,
	[Lastuser] [varchar](50) NULL,
	[LastUpdate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[PickingSeq] [numeric](18, 0) NOT NULL
) ON [PRIMARY]
GO
