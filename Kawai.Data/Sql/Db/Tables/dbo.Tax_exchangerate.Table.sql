SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Tax_exchangerate](
	[Exch_Year] [numeric](4, 0) NULL,
	[Exch_Month] [numeric](2, 0) NULL,
	[Week_Code] [numeric](1, 0) NULL,
	[Currency_Code] [char](2) NOT NULL,
	[Tax_ExchangeRate] [numeric](9, 2) NULL,
	[Start_Date] [char](8) NOT NULL,
	[End_Date] [char](8) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Tax_exchangerate] PRIMARY KEY CLUSTERED 
(
	[Currency_Code] ASC,
	[Start_Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
