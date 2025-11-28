SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [MS_PeriodSetting](
	[Period] [varchar](10) NOT NULL,
	[InventoryYear] [int] NOT NULL,
	[InventoryMonth] [int] NOT NULL,
	[StartPeriod] [datetime] NULL,
	[EndPeriod] [datetime] NULL,
	[StartSO] [datetime] NULL,
	[FinishSO] [datetime] NULL,
	[StatusStock] [varchar](50) NULL,
	[RegisterDate] [datetime] NULL,
	[RegisterUser] [varchar](35) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](35) NULL,
 CONSTRAINT [PK_MS_PeriodSetting] PRIMARY KEY CLUSTERED 
(
	[Period] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
