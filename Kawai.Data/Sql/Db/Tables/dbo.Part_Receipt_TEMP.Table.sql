SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Part_Receipt_TEMP](
	[Seq_No] [numeric](18, 0) NOT NULL,
	[Supplier_Code] [char](15) NOT NULL,
	[PO_No] [char](35) NOT NULL,
	[Warehouse_Code] [char](15) NOT NULL,
	[Address] [char](15) NULL,
	[Receipt_Cls] [char](2) NOT NULL,
	[MaterialConsump_Cls] [char](2) NULL,
	[Receipt_Date] [datetime] NOT NULL,
	[Item_Code] [char](25) NOT NULL,
	[Qty] [numeric](18, 5) NULL,
	[SerialNoFrom] [char](10) NULL,
	[SerialNoTo] [char](10) NULL,
	[Unit_Cls] [char](2) NULL,
	[Currency_Code] [char](2) NULL,
	[Price] [numeric](18, 5) NULL,
	[Price_Service] [numeric](18, 5) NULL,
	[Amount] [numeric](22, 5) NULL,
	[SuratJalan_No] [char](25) NULL,
	[ProductionResult_Cls] [char](1) NULL,
	[DailySeq_No] [numeric](18, 0) NULL,
	[Remarks] [char](100) NULL,
	[BC40_No] [char](30) NULL,
	[Transport_Cls] [char](2) NULL,
	[Package_Cls] [char](4) NULL,
	[Package_Qty] [numeric](8, 0) NULL,
	[Lot_No] [char](35) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[BC40_Date] [datetime] NULL,
	[BC_Type] [varchar](15) NULL
) ON [PRIMARY]
GO
