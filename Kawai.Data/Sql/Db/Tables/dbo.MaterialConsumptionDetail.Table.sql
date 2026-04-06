SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [MaterialConsumptionDetail](
	[ConsumptionID] [int] IDENTITY(1,1) NOT NULL,
	[ResultDetailID] [int] NOT NULL,
	[BarcodeMaterial] [varchar](50) NOT NULL,
	[MaterialCode] [varchar](25) NOT NULL,
	[MaterialName] [varchar](200) NULL,
	[LotNo] [varchar](50) NULL,
	[QtyUsed] [numeric](18, 9) NULL,
	[RegisterUser] [varchar](25) NULL,
	[RegisterDate] [datetime] NULL
) ON [PRIMARY]
GO
