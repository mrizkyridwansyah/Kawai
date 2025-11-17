SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Daily_Production](
	[Seq_No] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
	[Factory_code] [char](15) NOT NULL,
	[Line_Code] [char](15) NOT NULL,
	[Item_code] [char](25) NOT NULL,
	[Lot_No] [char](7) NOT NULL,
	[Qty] [numeric](18, 5) NULL,
	[SerialNoFrom] [char](10) NULL,
	[SerialNoTo] [char](10) NULL,
	[Unit_Cls] [char](2) NULL,
	[Schedule_Date] [datetime] NULL,
	[Remark] [char](50) NULL,
	[Request_Cls] [numeric](18, 0) NULL,
	[Complete_Cls] [char](1) NULL,
	[Auto_Cls] [char](1) NULL,
	[PlanCust_Code] [char](15) NULL,
	[PlanPO_No] [char](35) NULL,
	[PlanPO_SeqNo] [numeric](4, 0) NULL,
	[Plan_Seqno] [numeric](18, 0) NULL,
	[Prod_Barcode] [char](100) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Daily_Production] PRIMARY KEY CLUSTERED 
(
	[Seq_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [Daily_Production] ADD  CONSTRAINT [DF_Daily_Production_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
