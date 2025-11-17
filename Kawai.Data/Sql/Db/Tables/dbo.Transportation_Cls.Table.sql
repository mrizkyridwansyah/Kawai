SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Transportation_Cls](
	[Transportation_Cls] [char](2) NOT NULL,
	[Description] [char](25) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Transportation_Cls] PRIMARY KEY CLUSTERED 
(
	[Transportation_Cls] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [Transportation_Cls] ADD  CONSTRAINT [DF_Transportation_Cls_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
