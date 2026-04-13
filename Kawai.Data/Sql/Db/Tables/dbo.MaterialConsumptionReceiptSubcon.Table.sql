SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [MaterialConsumptionReceiptSubcon](
	[ConsumptionId] [bigint] IDENTITY(1,1) NOT NULL,
	[ReceiptId] [bigint] NOT NULL,
	[ReceiptDetailId] [bigint] NOT NULL,
	[PONumber] [varchar](100) NOT NULL,
	[ItemCode] [varchar](25) NOT NULL,
	[QtyReceipt] [numeric](18, 9) NULL,
	[RegisterUser] [varchar](25) NULL,
	[RegisterDate] [datetime] NULL
) ON [PRIMARY]
GO
