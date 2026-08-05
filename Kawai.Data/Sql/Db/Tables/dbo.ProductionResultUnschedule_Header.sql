USE [EZRunnerV3_KawaiLive]
GO

/****** Object:  Table [dbo].[ProductionResultUnschedule_Header]    Script Date: 8/5/2026 15:20:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductionResultUnschedule_Header](
	[ProdResultID] [bigint] IDENTITY(1,1) NOT NULL,
	[LineCode] [varchar](25) NULL,
	[ProductionDate] [date] NOT NULL,
	[ItemCode] [varchar](50) NOT NULL,
	[Shift] [varchar](10) NULL,
	[TotalGoodQty] [decimal](18, 2) NULL,
	[TotalNGQty] [decimal](18, 2) NULL,
	[Remarks] [nvarchar](max) NULL,
	[RegisterDate] [datetime] NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](50) NULL,
 CONSTRAINT [PK_ProductionResultUnschedule] PRIMARY KEY CLUSTERED 
(
	[ProdResultID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProductionResultUnschedule_Header] ADD  CONSTRAINT [DF_ProductionResultUnschedule_RegisterDate]  DEFAULT (getdate()) FOR [RegisterDate]
GO

ALTER TABLE [dbo].[ProductionResultUnschedule_Header] ADD  CONSTRAINT [DF_ProductionResultUnschedule_LastUpdate]  DEFAULT (getdate()) FOR [LastUpdate]
GO


