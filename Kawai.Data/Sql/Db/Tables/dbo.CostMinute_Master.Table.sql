SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [CostMinute_Master](
	[Factory_Code] [varchar](6) NOT NULL,
	[costminute_year] [char](4) NOT NULL,
	[costminute_month] [char](2) NOT NULL,
	[Line_Code] [char](6) NOT NULL,
	[Currency_Code] [char](2) NULL,
	[Cost_Minute] [numeric](18, 5) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_CostMinute_Master] PRIMARY KEY CLUSTERED 
(
	[Factory_Code] ASC,
	[costminute_year] ASC,
	[costminute_month] ASC,
	[Line_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
