USE [EZRunnerV3_KawaiLive]
GO

/****** Object:  Table [dbo].[ProductionResultUnschedule_Detail]    Script Date: 8/5/2026 15:21:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductionResultUnschedule_Detail](
	[ResultDetailID] [bigint] IDENTITY(1,1) NOT NULL,
	[ProdResultID] [bigint] NOT NULL,
	[BarcodeNo] [varchar](50) NOT NULL,
	[LotNo] [varchar](100) NOT NULL,
	[Qty] [numeric](18, 9) NOT NULL,
	[ResultType] [varchar](10) NULL,
	[Registerdate] [datetime] NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL,
	[PrintStatus] [varchar](1) NULL,
	[PrintDate] [datetime] NULL,
	[FlagConsump] [varchar](1) NULL,
	[FlagConsumpDate] [datetime] NULL,
 CONSTRAINT [PK_ProductionResultUnschedule_Detail] PRIMARY KEY CLUSTERED 
(
	[ResultDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProductionResultUnschedule_Detail] ADD  CONSTRAINT [DF_ProductionResultUnschedule_Detail_ResultType]  DEFAULT ('HOLD') FOR [ResultType]
GO

ALTER TABLE [dbo].[ProductionResultUnschedule_Detail] ADD  CONSTRAINT [DF_ProductionResultUnschedule_Detail_Registerdate]  DEFAULT (getdate()) FOR [Registerdate]
GO

ALTER TABLE [dbo].[ProductionResultUnschedule_Detail] ADD  CONSTRAINT [DF_ProductionResultUnschedule_Detail_LastUpdate]  DEFAULT (getdate()) FOR [LastUpdate]
GO


