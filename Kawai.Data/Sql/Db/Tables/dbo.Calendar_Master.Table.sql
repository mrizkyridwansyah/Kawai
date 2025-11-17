SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Calendar_Master](
	[Factory_Code] [char](15) NOT NULL,
	[Cal_Date] [datetime] NOT NULL,
	[Cal_Cls] [char](1) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Calendar_Master] PRIMARY KEY CLUSTERED 
(
	[Cal_Date] ASC,
	[Factory_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [Calendar_Master] ADD  CONSTRAINT [DF_Calendar_Master_Cal_Cls]  DEFAULT ((0)) FOR [Cal_Cls]
GO
ALTER TABLE [Calendar_Master] ADD  CONSTRAINT [DF_Calendar_Master_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
