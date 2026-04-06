SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ProductionResultDetail](
	[ResultDetailID] [bigint] IDENTITY(1,1) NOT NULL,
	[ProdResultID] [bigint] NOT NULL,
	[BarcodeNo] [varchar](50) NOT NULL,
	[LotNo] [varchar](100) NOT NULL,
	[SerialNo] [varchar](50) NULL,
	[Qty] [numeric](18, 9) NOT NULL,
	[ResultType] [varchar](10) NULL,
	[Registerdate] [datetime] NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL
) ON [PRIMARY]
GO
