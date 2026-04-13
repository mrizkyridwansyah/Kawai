SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Barcode_Split](
	[Warehouse_Code] [varchar](25) NOT NULL,
	[Area_Code] [varchar](25) NOT NULL,
	[Address_Code] [varchar](25) NOT NULL,
	[BarcodeNo] [char](35) NOT NULL,
	[Item_Code] [varchar](50) NOT NULL,
	[Lot_No] [varchar](50) NOT NULL,
	[SublotNo] [varchar](3) NULL,
	[Qty] [numeric](18, 9) NULL,
	[InventoryQty] [numeric](18, 9) NULL,
	[Print_Cls] [char](1) NULL,
	[Expired_Date] [date] NULL,
	[Production_Date] [date] NULL,
	[FactoryCode] [varchar](25) NULL,
	[ProcessCode] [varchar](25) NULL,
	[LineCode] [varchar](5) NULL,
	[Receipt_Date] [date] NULL,
	[Supplier] [varchar](150) NULL,
	[BarcodeNo_Original] [varchar](35) NULL,
	[RegisterDate] [datetime] NULL,
	[RegisterUser] [char](15) NULL,
	[Last_update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[CompleteStatus] [bit] NOT NULL,
	[IsAutomatic] [bit] NOT NULL,
	[LotNoNew] [varchar](50) NULL,
	[FromWarehouse] [varchar](25) NULL,
	[SourceNG] [bit] NULL
) ON [PRIMARY]
GO
