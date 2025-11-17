SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [IQC_Inspection_CheckPoint](
	[DetailID] [int] IDENTITY(1,1) NOT NULL,
	[InspectionID] [int] NOT NULL,
	[CheckItem] [varchar](100) NOT NULL,
	[Specification] [varchar](100) NULL,
	[MeasuredValue] [varchar](50) NULL,
	[Result] [varchar](10) NULL,
	[Remark] [text] NULL,
	[AttachmentPath] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[DetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [IQC_Inspection_CheckPoint]  WITH CHECK ADD  CONSTRAINT [FK_IQCDetail_Inspection] FOREIGN KEY([InspectionID])
REFERENCES [IQC_Inspection_Header] ([InspectionID])
GO
ALTER TABLE [IQC_Inspection_CheckPoint] CHECK CONSTRAINT [FK_IQCDetail_Inspection]
GO
ALTER TABLE [IQC_Inspection_CheckPoint]  WITH CHECK ADD CHECK  (([Result]='NG' OR [Result]='OK'))
GO
