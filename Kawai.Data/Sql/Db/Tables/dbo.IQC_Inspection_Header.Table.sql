SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [IQC_Inspection_Header](
	[InspectionID] [int] IDENTITY(1,1) NOT NULL,
	[PO_Number] [varchar](50) NULL,
	[ReceiptNo] [varchar](100) NULL,
	[SupplierCode] [char](15) NULL,
	[ItemCode] [varchar](50) NULL,
	[ItemName] [varchar](100) NULL,
	[InspectionDate] [datetime] NULL,
	[InspectorID] [char](15) NULL,
	[InspectionResult] [varchar](20) NULL,
	[InspectionResultApproval] [char](35) NULL,
	[InspectionResultDate] [datetime] NULL,
	[Remarks] [text] NULL,
	[RegisterDate] [datetime] NULL,
	[LastUpdate] [datetime] NULL,
	[TotalQtySample] [numeric](18, 2) NOT NULL,
	[TotalQtyNG] [numeric](18, 2) NULL,
	[Soruce] [varchar](20) NULL,
	[StatusQC] [varchar](10) NULL,
	[InspectionResultSADate] [datetime] NULL,
	[InspectionResultSA] [varchar](20) NULL,
	[InspectionResultSAApproval] [varchar](25) NULL,
	[RemarksSA] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[InspectionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [IQC_Inspection_Header] ADD  DEFAULT (getdate()) FOR [InspectionDate]
GO
ALTER TABLE [IQC_Inspection_Header] ADD  DEFAULT (getdate()) FOR [RegisterDate]
GO
ALTER TABLE [IQC_Inspection_Header] ADD  DEFAULT (getdate()) FOR [LastUpdate]
GO
ALTER TABLE [IQC_Inspection_Header] ADD  DEFAULT ((0)) FOR [TotalQtySample]
GO
ALTER TABLE [IQC_Inspection_Header]  WITH CHECK ADD CHECK  (([InspectionResult]='Hold' OR [InspectionResult]='Rejected' OR [InspectionResult]='Accepted' OR [InspectionResult]='SA'))
GO
ALTER TABLE [IQC_Inspection_Header]  WITH CHECK ADD CHECK  (([Soruce]='Material NG' OR [Soruce]='Incoming Material'))
GO
