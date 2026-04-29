SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PartReceiptDetailBarcodeTemp](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ReceiptDetailId] [bigint] NOT NULL,
	[ReceiptId] [bigint] NOT NULL,
	[ReceiptDate] [date] NOT NULL,
	[ReciptSeqNo] [int] NOT NULL,
	[PONumber] [varchar](50) NOT NULL,
	[ItemCode] [varchar](25) NOT NULL,
	[BarcodeNo] [varchar](50) NULL,
	[LotNo] [varchar](100) NULL,
	[SublotNo] [int] NULL,
	[Qty] [numeric](18, 9) NULL,
	[IsVerified] [bit] NULL,
	[VerifiedBy] [varchar](25) NULL,
	[VerifiedDate] [datetime] NULL,
	[PrintStatus] [bit] NULL,
	[PrintDate] [datetime] NULL,
	[PrintUser] [varchar](50) NULL,
	[WarehouseCode] [varchar](25) NULL,
	[ShippingLabelNo] [varchar](8) NULL,
) ON [PRIMARY]
GO
