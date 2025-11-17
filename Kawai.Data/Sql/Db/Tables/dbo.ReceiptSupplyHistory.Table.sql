SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ReceiptSupplyHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Status] [varchar](50) NULL,
	[ProcessMenu] [varchar](max) NULL,
	[RefNo] [varchar](50) NOT NULL,
	[WarehouseCode] [varchar](25) NOT NULL,
	[AreaCode] [varchar](25) NOT NULL,
	[AddressCode] [varchar](25) NOT NULL,
	[ItemCode] [varchar](60) NOT NULL,
	[BarcodeNo] [varchar](50) NOT NULL,
	[LotNo] [varchar](100) NOT NULL,
	[RefWarehouseCode] [varchar](25) NULL,
	[RefAreaCode] [varchar](25) NULL,
	[RefAddressCode] [varchar](25) NULL,
	[RefItemCode] [varchar](60) NULL,
	[RefBarcodeNo] [varchar](50) NULL,
	[RefLotNo] [varchar](100) NULL,
	[QtyTrans] [numeric](18, 9) NOT NULL,
	[Remarks] [varchar](max) NULL,
	[ReferenceNo] [varchar](100) NULL,
	[LogDate] [datetime] NULL,
	[UserID] [varchar](50) NULL,
 CONSTRAINT [PK_ReceiptSupplyHistory_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
