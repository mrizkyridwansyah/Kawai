SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Tax_Cls](
	[Tax_Code] [char](7) NOT NULL,
	[Tax_Name] [char](20) NULL,
	[Rate] [numeric](5, 2) NULL,
	[Start_Date] [char](8) NOT NULL,
	[End_Date] [char](8) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Tax_Cls] PRIMARY KEY CLUSTERED 
(
	[Tax_Code] ASC,
	[Start_Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
