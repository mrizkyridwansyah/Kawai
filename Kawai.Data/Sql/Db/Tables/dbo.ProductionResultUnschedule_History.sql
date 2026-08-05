USE [EZRunnerV3_KawaiLive]
GO

/****** Object:  Table [dbo].[ProductionResultUnschedule_History]    Script Date: 8/5/2026 15:21:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductionResultUnschedule_History](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ResultDetailID] [bigint] NOT NULL,
	[ParentItemCode] [varchar](50) NULL,
	[ItemCode] [varchar](50) NULL,
	[QtyBom] [numeric](18, 5) NULL,
	[QtyInput] [numeric](18, 5) NULL,
	[QtyRequirement] [numeric](18, 5) NULL,
	[QtyStock] [numeric](18, 5) NULL,
	[QtyUsed] [numeric](18, 5) NULL,
	[RegisterDate] [datetime] NULL,
	[RegisterUser] [nchar](10) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL,
 CONSTRAINT [PK_ProductionResultUnschedule_BOMRequire] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProductionResultUnschedule_History] ADD  CONSTRAINT [DF_ProductionResultUnschedule_BOMRequire_LastUpdate]  DEFAULT (getdate()) FOR [LastUpdate]
GO


