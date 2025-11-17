SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Invoice_Detail](
	[Invoice_No] [char](25) NOT NULL,
	[Packing_No] [char](25) NOT NULL,
	[PackingSeq_No] [numeric](4, 0) NOT NULL,
	[DO_No] [char](25) NOT NULL,
	[Item_Code] [char](25) NULL,
	[MakerItem_Code] [char](30) NULL,
	[Delivery_date] [datetime] NULL,
	[PO_No] [char](35) NOT NULL,
	[Seq_No] [numeric](4, 0) NOT NULL,
	[DOSeq_No] [numeric](4, 0) NOT NULL,
	[Qty] [numeric](18, 5) NULL,
	[Price] [numeric](18, 5) NULL,
	[service] [numeric](18, 5) NULL,
	[Currency_Code] [char](2) NULL,
	[Unit_Cls] [char](2) NULL,
	[Amount] [numeric](22, 5) NULL,
	[ExchangeRate_Amount] [numeric](22, 5) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_Invoice_Detail] PRIMARY KEY CLUSTERED 
(
	[Invoice_No] ASC,
	[Packing_No] ASC,
	[PackingSeq_No] ASC,
	[DO_No] ASC,
	[PO_No] ASC,
	[Seq_No] ASC,
	[DOSeq_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [Invoice_Detail] ADD  CONSTRAINT [DF_Invoice_Detail_DOSeq_No]  DEFAULT ((0)) FOR [DOSeq_No]
GO
ALTER TABLE [Invoice_Detail] ADD  CONSTRAINT [DF_Invoice_Detail_Register_Date]  DEFAULT (getdate()) FOR [Register_Date]
GO
