SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TPrice_Master](
	[Price_Cls] [nvarchar](255) NULL,
	[Item_Code] [nvarchar](255) NULL,
	[Trade_Code] [nvarchar](255) NULL,
	[Priority_Cls] [nvarchar](255) NULL,
	[Currency_Code] [nvarchar](255) NULL,
	[Price] [nvarchar](255) NULL,
	[Unit_Cls] [nvarchar](255) NULL,
	[Start_Date] [nvarchar](255) NULL,
	[End_Date] [nvarchar](255) NULL,
	[Reason_Cls] [nvarchar](255) NULL,
	[Remarks] [nvarchar](255) NULL,
	[Last_Update] [nvarchar](255) NULL,
	[Last_User] [nvarchar](255) NULL,
	[Register_Date] [nvarchar](255) NULL
) ON [PRIMARY]
GO
