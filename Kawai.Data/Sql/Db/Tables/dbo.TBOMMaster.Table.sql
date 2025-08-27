SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TBOMMaster](
	[Parent_ItemCode] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[Item_Code] [nvarchar](255) NULL,
	[Item_Name] [nvarchar](255) NULL,
	[Qty] [nvarchar](255) NULL,
	[R_Qty] [nvarchar](255) NULL,
	[Unit_Cls] [nvarchar](255) NULL,
	[Currency_Code] [nvarchar](255) NULL,
	[Accounting_Code] [nvarchar](255) NULL,
	[Start_Date] [nvarchar](255) NULL,
	[End_date] [nvarchar](255) NULL,
	[Last_Update] [nvarchar](255) NULL,
	[Last_User] [nvarchar](255) NULL,
	[Register_Date] [nvarchar](255) NULL,
	[Doc_No] [nvarchar](255) NULL,
	[Revision_No] [nvarchar](255) NULL,
	[Field17] [nvarchar](255) NULL
) ON [PRIMARY]
GO
