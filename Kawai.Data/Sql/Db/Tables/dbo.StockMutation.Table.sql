SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [StockMutation](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[WarehouseCode] [varchar](25) NOT NULL,
	[AreaCode] [varchar](25) NOT NULL,
	[AddressCode] [varchar](25) NOT NULL,
	[ItemCode] [varchar](25) NOT NULL,
	[BarcodeNo] [varchar](50) NULL,
	[LotNo] [varchar](100) NULL,
	[QtyTrans] [numeric](18, 9) NOT NULL,
	[InventoryQty] [numeric](18, 9) NULL,
	[SublotNo] [int] NULL,
	[SourceType] [varchar](50) NULL,
	[SourceRef] [varchar](50) NULL,
	[SourceRefNo] [varchar](100) NULL,
	[RefMutationId] [bigint] NULL,
	[HasCalculate] [bit] NULL,
	[StatusCalculate] [varchar](10) NULL,
	[ErrorMessage] [varchar](max) NULL,
	[RegisterDate] [datetime] NOT NULL,
	[RegisterUser] [varchar](25) NOT NULL,
	[CalculateDate] [datetime] NULL,
 CONSTRAINT [PK_StockMutation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
