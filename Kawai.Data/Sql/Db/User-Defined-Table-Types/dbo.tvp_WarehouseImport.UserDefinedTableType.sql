CREATE TYPE [tvp_WarehouseImport] AS TABLE(
	[FactoryCode] [varchar](25) NOT NULL,
	[WarehouseCode] [varchar](25) NOT NULL,
	[WarehouseName] [varchar](100) NOT NULL,
	[AdmGroup] [varchar](15) NOT NULL,
	[StockControlCls] [varchar](2) NOT NULL,
	[NGCls] [varchar](2) NOT NULL,
	[UseEndDate] [date] NULL,
	[RowNumber] [int] NULL,
	[Errors] [varchar](max) NULL
)
GO
