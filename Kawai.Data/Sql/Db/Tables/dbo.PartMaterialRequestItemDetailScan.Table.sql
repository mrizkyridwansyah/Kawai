SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PartMaterialRequestItemDetailScan](
	[IDScan] [bigint] IDENTITY(1,1) NOT NULL,
	[IDSeq] [bigint] NOT NULL,
	[FromRefNo] [varchar](50) NOT NULL,
	[FromWarehouseCode] [varchar](25) NOT NULL,
	[FromAreaCode] [varchar](25) NOT NULL,
	[FromAddressCode] [varchar](25) NOT NULL,
	[ToRefNo] [varchar](50) NOT NULL,
	[ToWarehouseCode] [varchar](25) NOT NULL,
	[ToAreaCode] [varchar](25) NOT NULL,
	[ToAddressCode] [varchar](25) NOT NULL,
	[BarcodeNo] [varchar](50) NOT NULL,
	[ItemCode] [char](25) NOT NULL,
	[LotNo] [varchar](100) NOT NULL,
	[Qty] [numeric](18, 9) NULL,
	[Remarks] [varchar](255) NULL,
	[RegisterDate] [datetime] NOT NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL,
PRIMARY KEY CLUSTERED 
(
	[IDScan] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [PartMaterialRequestItemDetailScan]  WITH CHECK ADD FOREIGN KEY([IDSeq])
REFERENCES [PartMaterialRequestItemDetail] ([IDSeq])
GO
ALTER TABLE [PartMaterialRequestItemDetailScan]  WITH CHECK ADD FOREIGN KEY([ItemCode])
REFERENCES [Item_Master] ([Item_Code])
GO
