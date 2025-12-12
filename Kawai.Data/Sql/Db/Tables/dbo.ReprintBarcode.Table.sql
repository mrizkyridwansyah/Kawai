SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReprintBarcode](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[WarehouseCode] [varchar](25) NOT NULL,
	[AreaCode] [varchar](25) NOT NULL,
	[AddressCode] [varchar](25) NOT NULL,
	[BarcodeNo] [varchar](50) NOT NULL,
	[LotNo] [varchar](100) NOT NULL,
	[Qty] [numeric](18, 9) NOT NULL,
	[RegisterDate] [datetime] NOT NULL,
	[RegisterUser] [varchar](25) NOT NULL,
	[PrintStatus] [bit] NOT NULL,
	[PrintDate] [datetime] NULL,
	[SourceMenu] [varchar](100) NULL,
 CONSTRAINT [PK_ReprintBarcode] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

