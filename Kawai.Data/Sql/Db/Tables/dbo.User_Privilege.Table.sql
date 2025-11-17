SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [User_Privilege](
	[App_ID] [char](3) NOT NULL,
	[UserName] [char](15) NOT NULL,
	[Menu_ID] [char](10) NOT NULL,
	[Allow_Update] [char](1) NULL,
	[Allow_Price] [char](1) NULL,
	[Status] [char](1) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_User_Privilege] PRIMARY KEY CLUSTERED 
(
	[App_ID] ASC,
	[UserName] ASC,
	[Menu_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [User_Privilege] ADD  CONSTRAINT [DF_User_Privilege_Allow_Update]  DEFAULT ((0)) FOR [Allow_Update]
GO
ALTER TABLE [User_Privilege] ADD  CONSTRAINT [DF_User_Privilege_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
ALTER TABLE [User_Privilege]  WITH NOCHECK ADD  CONSTRAINT [FK_User_Privilege_User_Menu] FOREIGN KEY([App_ID], [Menu_ID])
REFERENCES [User_Menu] ([App_ID], [Menu_ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [User_Privilege] CHECK CONSTRAINT [FK_User_Privilege_User_Menu]
GO
ALTER TABLE [User_Privilege]  WITH NOCHECK ADD  CONSTRAINT [FK_User_Privilege_User_Setup] FOREIGN KEY([App_ID], [UserName])
REFERENCES [User_Setup] ([App_ID], [Username])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [User_Privilege] CHECK CONSTRAINT [FK_User_Privilege_User_Setup]
GO
