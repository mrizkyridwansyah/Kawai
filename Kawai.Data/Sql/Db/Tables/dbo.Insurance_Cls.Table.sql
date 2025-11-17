SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Insurance_Cls](
	[Insurance_Cls] [char](2) NULL,
	[Description] [char](25) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL
) ON [PRIMARY]
GO
ALTER TABLE [Insurance_Cls] ADD  CONSTRAINT [DF_Insurance_Cls_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
