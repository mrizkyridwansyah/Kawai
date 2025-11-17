SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [IQC_Attachment](
	[AttachmentID] [int] IDENTITY(1,1) NOT NULL,
	[InspectionID] [int] NOT NULL,
	[FileName] [varchar](255) NULL,
	[FilePath] [varchar](255) NULL,
	[RegisterUser] [int] NULL,
	[RegisterDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[AttachmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [IQC_Attachment] ADD  DEFAULT (getdate()) FOR [RegisterDate]
GO
ALTER TABLE [IQC_Attachment]  WITH CHECK ADD  CONSTRAINT [FK_IQCAttachment_Inspection] FOREIGN KEY([InspectionID])
REFERENCES [IQC_Inspection_Header] ([InspectionID])
GO
ALTER TABLE [IQC_Attachment] CHECK CONSTRAINT [FK_IQCAttachment_Inspection]
GO
