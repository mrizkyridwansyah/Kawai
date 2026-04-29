CREATE TYPE [dbo].[ProductionQualityJudgementType] AS TABLE(
	[ProductionId] [bigint] NULL,
	[ProdResultID] [varchar](50) NULL,
	[ResultType] [varchar](20) NULL,
	[ItemCode] [varchar](50) NULL
)
GO
