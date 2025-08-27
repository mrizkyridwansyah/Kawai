SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Supply_Scan_Header](
	[SJ_No] [char](25) NOT NULL,
	[SJ_Date] [date] NULL,
	[From_WH] [char](15) NULL,
	[To_WH] [char](15) NULL,
	[Receipt_Status] [varchar](1) NULL,
	[Receipt_User] [char](25) NULL,
	[Receipt_Date] [datetime] NULL,
	[BC_Type] [varchar](15) NULL,
	[BC40_No] [varchar](30) NULL,
	[BC40_Date] [datetime] NULL,
	[IP_Address] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[Registre_User] [char](25) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](25) NULL,
 CONSTRAINT [PK_Supply_Scan_Header] PRIMARY KEY CLUSTERED 
(
	[SJ_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
