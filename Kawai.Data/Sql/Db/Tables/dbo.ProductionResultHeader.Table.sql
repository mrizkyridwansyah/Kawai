SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ProductionResultHeader](
	[ProdResultID] [bigint] IDENTITY(1,1) NOT NULL,
	[ProductionID] [numeric](18, 0) NOT NULL,
	[ProductionDate] [date] NOT NULL,
	[ItemCode] [char](25) NOT NULL,
	[Shift] [varchar](10) NULL,
	[TotalGoodQty] [decimal](18, 2) NULL,
	[TotalNGQty] [decimal](18, 2) NULL,
	[RegisterDate] [datetime] NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](50) NULL
) ON [PRIMARY]
GO
