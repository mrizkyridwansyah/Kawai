SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoadingConfirmationEvidence](
	[Evidence_No] [varchar](50) NOT NULL,
	[Loading_No] [varchar](50) NOT NULL,
	[Evidence_Type] [varchar](20) NOT NULL,
	[Container_No] [varchar](50) NOT NULL,
	[Vehicle_No] [varchar](50) NOT NULL,
	[Seal_No] [varchar](50) NOT NULL,
	[Revision_No] [int] NULL,
	[Remark] [varchar](max) NULL,
	[Driver_Name] [varchar](100) NULL,
	[Transport_Vendor] [varchar](100) NULL,
	[Latitude] [varchar](50) NULL,
	[Longitude] [varchar](50) NULL,
	[Evidence_Date] [datetime] NOT NULL,
	[Register_Date] [datetime] NOT NULL,
	[Register_By] [varchar](50) NOT NULL,
	[Is_Deleted] [int] NULL,
	[Deleted_Date] [datetime] NULL,
	[Deleted_By] [varchar](50) NULL,
	[Update_Date] [datetime] NULL,
	[Update_By] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Evidence_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[LoadingConfirmationEvidence] ADD  DEFAULT (getdate()) FOR [Evidence_Date]
GO

ALTER TABLE [dbo].[LoadingConfirmationEvidence] ADD  DEFAULT (getdate()) FOR [Register_Date]
GO


