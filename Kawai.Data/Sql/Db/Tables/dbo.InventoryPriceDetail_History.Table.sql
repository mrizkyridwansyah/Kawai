SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [InventoryPriceDetail_History](
	[Period] [char](6) NOT NULL,
	[Item_Code] [char](15) NOT NULL,
	[Tgl] [datetime] NOT NULL,
	[Idx] [char](1) NOT NULL,
	[Cls] [char](3) NOT NULL,
	[Seq_No] [numeric](18, 0) NOT NULL,
	[Duty_Status] [char](1) NOT NULL,
	[From_Loc] [char](6) NULL,
	[To_Loc] [char](6) NULL,
	[PO_No] [char](25) NULL,
	[DO_No] [char](25) NULL,
	[Lot_No] [char](7) NULL,
	[QtyOP] [numeric](18, 5) NULL,
	[QtyIn] [numeric](18, 5) NULL,
	[QtyInOt] [numeric](18, 5) NULL,
	[QtyOut] [numeric](18, 5) NULL,
	[QtyOutOt] [numeric](18, 5) NULL,
	[QtyEnding] [numeric](18, 5) NULL,
	[Currency_Code] [char](2) NULL,
	[Price] [numeric](18, 5) NULL,
	[AvgPrice] [numeric](18, 5) NULL,
	[Amount] [numeric](22, 5) NULL,
	[AmountEnding] [numeric](22, 5) NULL,
	[Last_User] [char](15) NULL,
	[Last_Update] [datetime] NULL,
	[Register_Date] [datetime] NULL,
	[Company_Code] [char](5) NOT NULL,
 CONSTRAINT [PK_InventoryPriceDetail_History] PRIMARY KEY CLUSTERED 
(
	[Period] ASC,
	[Item_Code] ASC,
	[Tgl] ASC,
	[Idx] ASC,
	[Cls] ASC,
	[Seq_No] ASC,
	[Duty_Status] ASC,
	[Company_Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
