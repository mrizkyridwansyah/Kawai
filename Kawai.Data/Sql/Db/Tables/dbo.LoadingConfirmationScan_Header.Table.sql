SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoadingConfirmationScan_Header](
	[Loading_No] [varchar](50) NOT NULL,
	[SI_No] [varchar](50) NOT NULL,
	[WH_Code] [varchar](25) NULL,
	[Container_No] [varchar](50) NULL,
	[Vehicle_No] [varchar](50) NULL,
	[Seal_No] [varchar](50) NULL,
	[Loading_Status] [varchar](25) NULL,
	[Before_Status] [bit] NOT NULL,
	[After_Status] [bit] NOT NULL,
	[Start_Date] [datetime] NOT NULL,
	[Finish_Date] [datetime] NULL,
	[Register_Date] [datetime] NOT NULL,
	[Register_By] [varchar](50) NOT NULL,
	[Update_Date] [datetime] NULL,
	[Update_By] [varchar](50) NULL,
	[AllScanned_Status] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Loading_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LoadingConfirmationScan_Header] ADD  DEFAULT ('OPEN') FOR [Loading_Status]
GO

ALTER TABLE [dbo].[LoadingConfirmationScan_Header] ADD  DEFAULT ((0)) FOR [Before_Status]
GO

ALTER TABLE [dbo].[LoadingConfirmationScan_Header] ADD  DEFAULT ((0)) FOR [After_Status]
GO

ALTER TABLE [dbo].[LoadingConfirmationScan_Header] ADD  DEFAULT (getdate()) FOR [Start_Date]
GO

ALTER TABLE [dbo].[LoadingConfirmationScan_Header] ADD  DEFAULT (getdate()) FOR [Register_Date]
GO


