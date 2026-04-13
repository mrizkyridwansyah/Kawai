SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [MaterialConsumptionReceiptSubconDetail](
	[ConsumptionDetailId] [bigint] IDENTITY(1,1) NOT NULL,
	[ConsumptionId] [bigint] NOT NULL,
	[BarcodeNo] [varchar](50) NOT NULL,
	[ItemCode] [varchar](25) NOT NULL,
	[QtyUsed] [numeric](18, 9) NULL,
	[RegisterUser] [varchar](25) NULL,
	[RegisterDate] [datetime] NULL
) ON [PRIMARY]
GO
