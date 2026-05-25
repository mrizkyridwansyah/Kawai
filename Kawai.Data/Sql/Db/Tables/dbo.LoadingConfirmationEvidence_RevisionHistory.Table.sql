SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoadingConfirmationEvidence_RevisionHistory](
	[History_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Evidence_No] [varchar](50) NULL,
	[Field_Name] [varchar](50) NULL,
	[Old_Value] [varchar](max) NULL,
	[New_Value] [varchar](max) NULL,
	[Revision_Reason] [varchar](500) NULL,
	[Change_Date] [datetime] NULL,
	[Change_By] [varchar](50) NULL,
	[Change_Type] [varchar](50) NULL,
	[Revision_No] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[LoadingConfirmationEvidence_RevisionHistory] ADD  DEFAULT (getdate()) FOR [Change_Date]
GO


