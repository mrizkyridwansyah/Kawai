SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [PO_Set_Master](
	[Parent_SetCode] [char](25) NOT NULL,
	[Description] [char](50) NULL,
	[Item_Code] [char](25) NOT NULL,
	[Item_Name] [char](75) NULL,
	[Qty] [numeric](18, 2) NULL,
	[Register_Date] [datetime] NULL,
	[Register_User] [char](15) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
 CONSTRAINT [PK_PO_Set_Master] PRIMARY KEY CLUSTERED 
(
	[Parent_SetCode] ASC,
	[Item_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
