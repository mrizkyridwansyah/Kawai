SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Closing_Receipt](
	[Closing_Year] [numeric](4, 0) NOT NULL,
	[Closing_Month] [numeric](2, 0) NOT NULL,
	[Closing_Date] [datetime] NULL,
	[Status] [char](1) NULL
) ON [PRIMARY]
GO
