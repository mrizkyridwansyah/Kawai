SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [App_FactoryPrivilege](
	[UserID] [varchar](15) NOT NULL,
	[Factory_Code] [varchar](15) NOT NULL,
	[Show] [varchar](1) NULL,
	[RegisterUser] [varchar](15) NULL,
	[RegisterDate] [datetime] NULL,
	[UpdateUser] [varchar](50) NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_App_FactoryPrivilege] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[Factory_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
