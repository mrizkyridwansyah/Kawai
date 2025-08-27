SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [UT_User_Setup](
	[AppID] [nvarchar](3) NOT NULL,
	[UserID] [nvarchar](15) NOT NULL,
	[UserName] [nvarchar](25) NULL,
	[UserType] [nvarchar](3) NULL,
	[Password] [nvarchar](100) NULL,
	[AdminStatus] [nvarchar](1) NULL,
	[Lock_Cls] [nvarchar](2) NULL,
	[Description] [nvarchar](25) NULL,
	[FailedLogin] [int] NULL,
	[User_Email] [nvarchar](50) NULL,
	[CreateDate] [datetime] NULL,
	[CreateUser] [nvarchar](15) NULL,
	[UpdateDate] [datetime] NULL,
	[UpdateUser] [nvarchar](15) NULL,
 CONSTRAINT [PK_User_Setup1] PRIMARY KEY CLUSTERED 
(
	[AppID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
