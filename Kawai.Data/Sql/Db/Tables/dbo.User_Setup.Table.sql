SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [User_Setup](
	[App_ID] [char](3) NOT NULL,
	[Username] [char](15) NOT NULL,
	[Name] [char](25) NULL,
	[Password] [char](250) NULL,
	[Description] [char](25) NULL,
	[Locked] [char](1) NULL,
	[InvalidLogin] [numeric](3, 0) NULL,
	[InitPO] [char](1) NULL,
	[Status_Admin] [char](1) NULL,
	[Last_Login] [datetime] NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_User_Setup] PRIMARY KEY CLUSTERED 
(
	[App_ID] ASC,
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
