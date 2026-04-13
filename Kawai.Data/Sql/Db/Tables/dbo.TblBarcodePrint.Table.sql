SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TblBarcodePrint](
	[BarcodeNo] [varchar](100) NOT NULL,
	[Item_Code] [varchar](150) NULL,
	[Item_Name] [varchar](150) NULL,
	[SupplierName] [varchar](150) NULL,
	[CompanyName] [varchar](150) NULL,
	[PO_No] [varchar](150) NULL,
	[DN_No] [varchar](150) NULL,
	[Lot_No] [varchar](150) NULL,
	[Qty] [varchar](150) NULL,
	[Unit] [varchar](150) NULL,
	[Production_Date] [varchar](150) NULL,
	[Delivery_Date] [varchar](150) NULL,
	[Expired] [varchar](150) NULL,
	[Printer_Name] [varchar](150) NULL,
	[IP_Printer_Address] [varchar](150) NULL,
	[SublotNo] [varchar](150) NULL,
	[Operator] [varchar](150) NULL,
	[RakNumb] [varchar](150) NULL,
	[Remarks] [varchar](150) NULL,
	[LineCode] [varchar](150) NULL,
	[Additional_Info_01] [varchar](150) NULL,
	[Additional_Info_02] [varchar](150) NULL,
	[Additional_Info_03] [varchar](150) NULL,
	[Additional_Info_04] [varchar](150) NULL,
	[Additional_Info_05] [varchar](150) NULL,
	[Additional_Info_06] [varchar](150) NULL,
	[BarcodeTitle] [varchar](150) NULL,
	[SourceData] [varchar](150) NOT NULL
) ON [PRIMARY]
GO
