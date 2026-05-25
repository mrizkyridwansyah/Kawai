SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoadingConfirmationEvidence_Attachment](
	[Evidence_No] [varchar](50) NOT NULL,
	[SeqNo] [int] NOT NULL,
	[File_Name] [varchar](255) NOT NULL,
	[File_Path] [varchar](500) NOT NULL,
	[File_Extension] [varchar](20) NULL,
	[Register_Date] [datetime] NOT NULL,
	[Register_By] [varchar](50) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Delete_Date] [datetime] NULL,
	[Delete_By] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Evidence_No] ASC,
	[SeqNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LoadingConfirmationEvidence_Attachment] ADD  DEFAULT (getdate()) FOR [Register_Date]
GO

ALTER TABLE [dbo].[LoadingConfirmationEvidence_Attachment] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO


