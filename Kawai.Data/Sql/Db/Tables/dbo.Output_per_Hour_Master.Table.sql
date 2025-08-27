SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Output_per_Hour_Master](
	[Model_Cls] [char](2) NULL,
	[Description] [char](25) NULL,
	[Output_per_hour] [numeric](3, 1) NULL,
	[Jumlah_Operator] [numeric](3, 1) NULL
) ON [PRIMARY]
GO
