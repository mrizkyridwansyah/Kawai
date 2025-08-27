SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Part_Supply_TEMP](
	[Seq_No] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
	[SupplyRec_No] [char](25) NULL,
	[RecSeq_No] [numeric](18, 0) NULL,
	[FromWarehouse_Code] [char](15) NOT NULL,
	[From_Address] [char](15) NOT NULL,
	[ToWarehouse_Code] [char](15) NOT NULL,
	[ChildSupply_date] [datetime] NULL,
	[ChildItem_Code] [char](25) NOT NULL,
	[Supply_Cls] [char](2) NOT NULL,
	[ChildRequirement_Qty] [numeric](18, 5) NULL,
	[Consumption_Qty] [numeric](18, 5) NULL,
	[ChildUnit_Cls] [char](2) NULL,
	[Currency_Code] [char](2) NULL,
	[Price] [numeric](18, 5) NULL,
	[Service] [numeric](18, 5) NULL,
	[Amount] [numeric](22, 2) NULL,
	[ParentItem_Code] [char](25) NULL,
	[Lot_No] [char](7) NULL,
	[Production_Date] [datetime] NULL,
	[DO_No] [char](25) NOT NULL,
	[Remarks] [char](35) NULL,
	[SJNo] [char](20) NULL,
	[MaterialConsump_Cls] [char](2) NULL,
	[SubConPartReceipt_SeqNo] [char](18) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
	[SupplySeq_No] [numeric](18, 0) NULL,
	[BC_Type] [varchar](15) NULL,
	[BC40_No] [varchar](30) NULL,
	[BC40_Date] [datetime] NULL,
	[DailySeqno] [numeric](18, 0) NULL
) ON [PRIMARY]
GO
