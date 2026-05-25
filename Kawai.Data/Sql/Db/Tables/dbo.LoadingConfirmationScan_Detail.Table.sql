SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LoadingConfirmationScan_Detail](
	[Loading_No] [varchar](50) NOT NULL,
	[SeqNo] [int] NOT NULL,
	[SI_No] [varchar](50) NOT NULL,
	[PO_No] [varchar](35) NULL,
	[PO_SeqNo] [int] NULL,
	[Item_Code] [varchar](50) NOT NULL,
	[Serial_No] [varchar](100) NOT NULL,
	[Barcode_No] [varchar](100) NOT NULL,
	[WarehouseCode] [varchar](25) NULL,
	[AreaCode] [varchar](25) NULL,
	[AddressCode] [varchar](25) NULL,
	[Scan_Status] [varchar](20) NOT NULL,
	[Scan_Date] [datetime] NOT NULL,
	[Scan_By] [varchar](50) NOT NULL,
	[Submit_Status] [bit] NOT NULL,
	[Device_ID] [varchar](100) NULL,
 CONSTRAINT [PK_LoadingConfirmationScanDetail] PRIMARY KEY CLUSTERED 
(
	[Loading_No] ASC,
	[SeqNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LoadingConfirmationScan_Detail] ADD  DEFAULT ('VALID') FOR [Scan_Status]
GO

ALTER TABLE [dbo].[LoadingConfirmationScan_Detail] ADD  DEFAULT (getdate()) FOR [Scan_Date]
GO

ALTER TABLE [dbo].[LoadingConfirmationScan_Detail] ADD  DEFAULT ((0)) FOR [Submit_Status]
GO


