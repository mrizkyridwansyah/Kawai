CREATE TYPE [tvp_PartMaterialRequestWomin] AS TABLE(
	[RequestId] [bigint] NULL,
	[ProductionId] [bigint] NULL,
	[ScheduleDate] [date] NULL,
	[ItemCode] [varchar](25) NULL,
	[RequestSetQty] [numeric](18, 9) NULL
)
GO
