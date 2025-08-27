SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [FakturPajak_Detail](
	[FakturPajak_No] [char](25) NOT NULL,
	[Invoice_No] [char](25) NOT NULL,
	[DO_No] [char](25) NOT NULL,
	[Packing_No] [char](25) NOT NULL,
	[PackingSeq_No] [numeric](4, 0) NOT NULL,
	[Item_Code] [char](25) NOT NULL,
	[MakerItem_Code] [char](30) NULL,
	[Delivery_Date] [datetime] NOT NULL,
	[PO_No] [char](35) NOT NULL,
	[Seq_No] [numeric](4, 0) NOT NULL,
	[DOSeq_No] [numeric](4, 0) NOT NULL,
	[Qty] [numeric](18, 5) NULL,
	[Price] [numeric](18, 5) NULL,
	[Currency_Code] [char](2) NULL,
	[Unit_Cls] [char](2) NULL,
	[Amount] [numeric](22, 5) NULL,
	[Exhange_Rate] [numeric](9, 2) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_FakturPajak_Detail] PRIMARY KEY CLUSTERED 
(
	[FakturPajak_No] ASC,
	[Invoice_No] ASC,
	[DO_No] ASC,
	[PO_No] ASC,
	[Seq_No] ASC,
	[DOSeq_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [FakturPajak_Detail]  WITH NOCHECK ADD  CONSTRAINT [FK_FakturPajak_Detail_FakturPajak_Master] FOREIGN KEY([FakturPajak_No])
REFERENCES [FakturPajak_Master] ([FakturPajak_No])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [FakturPajak_Detail] CHECK CONSTRAINT [FK_FakturPajak_Detail_FakturPajak_Master]
GO
