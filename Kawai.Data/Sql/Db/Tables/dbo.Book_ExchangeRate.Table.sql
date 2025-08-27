SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Book_ExchangeRate](
	[Exch_Year] [decimal](4, 0) NOT NULL,
	[Term_Cls] [char](1) NOT NULL,
	[Currency_Code] [char](2) NOT NULL,
	[Exch01] [decimal](9, 2) NULL,
	[Exch02] [decimal](9, 2) NULL,
	[Exch03] [decimal](9, 2) NULL,
	[Exch04] [decimal](9, 2) NULL,
	[Exch05] [decimal](9, 2) NULL,
	[Exch06] [decimal](9, 2) NULL,
	[Exch07] [decimal](9, 2) NULL,
	[Exch08] [decimal](9, 2) NULL,
	[Exch09] [decimal](9, 2) NULL,
	[Exch010] [decimal](9, 2) NULL,
	[Exch011] [decimal](9, 2) NULL,
	[Exch012] [decimal](9, 2) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL
) ON [PRIMARY]
GO
