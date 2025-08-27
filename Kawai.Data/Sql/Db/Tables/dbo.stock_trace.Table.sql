SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [stock_trace](
	[register_date] [datetime] NULL,
	[ope_date] [datetime] NULL,
	[Warehouse_Code] [char](15) NULL,
	[item_code] [char](25) NULL,
	[trace_log] [varchar](2000) NULL
) ON [PRIMARY]
GO
