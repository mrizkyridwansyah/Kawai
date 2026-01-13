CREATE TYPE [tvp_PeriodSettingDetail] AS TABLE(
	[Period] [varchar](50) NULL,
	[Year] [int] NULL,
	[Month] [int] NULL,
	[MonthName] [varchar](50) NULL,
	[StartPeriod] [datetime] NULL,
	[EndPeriod] [datetime] NULL,
	[StartSO] [datetime] NULL,
	[FinishSO] [datetime] NULL
)
GO
