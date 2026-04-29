CREATE TYPE [dbo].[tvp_ProductionResultManualInput] AS TABLE(
	[ProdResultID] [bigint] NULL,
	[ProductionId] [bigint] NULL,
	[ScheduleDate] [date] NULL,
	[ItemCode] [varchar](25) NULL,
	[ResultQty] [numeric](18, 9) NULL
)
GO


