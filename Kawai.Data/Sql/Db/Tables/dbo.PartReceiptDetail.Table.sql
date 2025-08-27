SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PartReceiptDetail](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ReceiptId] [bigint] NOT NULL,
	[ReceiptDate] [date] NOT NULL,
	[PONumber] [varchar](50) NULL,
	[ItemCode] [varchar](25) NULL,
	[UnitCls] [varchar](2) NULL,
	[ExpectedQty] [numeric](18, 9) NULL,
	[TotalPacking] [int] NULL,
	[ReceiptQty] [numeric](18, 9) NULL,
	[IQCResult] [varchar](10) NULL,
	[Remarks] [varchar](max) NULL,
	[Attachment] [varchar](200) NULL,
	[InspectionDate] [datetime] NULL,
	[InspectionBy] [varchar](25) NULL,
	[HasValid] [bit] NULL,
	[ValidDate] [datetime] NULL,
 CONSTRAINT [PK_PartReceiptDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
