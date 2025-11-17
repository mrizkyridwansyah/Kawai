SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [IQC_Header](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[SourceData] [varchar](100) NULL,
	[DNNumber] [varchar](50) NULL,
	[SupplierCode] [varchar](15) NULL,
	[PONumber] [varchar](50) NULL,
	[ItemCode] [varchar](25) NULL,
	[ReceiptDate] [datetime] NULL,
	[TotalQty] [numeric](18, 9) NULL,
	[QCStatus] [varchar](25) NULL,
	[QCPhoto] [varchar](25) NULL,
	[Remarks] [varchar](200) NULL,
	[InspectionDate] [datetime] NULL,
	[InspectionUser] [varchar](25) NULL,
	[RegisterDate] [datetime] NULL,
	[RegisterUser] [varchar](25) NULL,
	[LastUpdate] [datetime] NULL,
	[LastUser] [varchar](25) NULL,
 CONSTRAINT [PK_IQC_Header] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
