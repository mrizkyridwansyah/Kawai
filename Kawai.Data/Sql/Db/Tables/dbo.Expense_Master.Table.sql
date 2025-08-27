SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Expense_Master](
	[Year] [char](4) NULL,
	[Month] [char](2) NULL,
	[Expense] [numeric](18, 5) NULL,
	[Factor] [numeric](18, 6) NULL
) ON [PRIMARY]
GO
