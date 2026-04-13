SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TblBarcodePrint_Update](
	[BarcodeNo] [varchar](100) NOT NULL,
	[PrintStatus] [varchar](10) NULL,
	[PrintDate] [datetime] NULL,
	[PrintUser] [varchar](100) NULL,
	[SourceData] [varchar](150) NOT NULL
) ON [PRIMARY]
GO
