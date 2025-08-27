SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [FakturPajak_Master](
	[Cust_Code] [char](15) NULL,
	[FakturPajak_No] [char](25) NOT NULL,
	[FakturPajak_Date] [datetime] NOT NULL,
	[Amount] [numeric](22, 5) NULL,
	[PPN] [numeric](22, 5) NULL,
	[Total_Amount] [numeric](22, 5) NULL,
	[Amount_IDR] [numeric](22, 5) NULL,
	[PPN_IDR] [numeric](22, 5) NULL,
	[Fix_Cls] [char](1) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_FakturPajak_Master] PRIMARY KEY CLUSTERED 
(
	[FakturPajak_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [FakturPajak_Master]  WITH NOCHECK ADD  CONSTRAINT [FK_FakturPajak_Master_Trade_Master] FOREIGN KEY([Cust_Code])
REFERENCES [Trade_Master] ([Trade_Code])
ON UPDATE CASCADE
GO
ALTER TABLE [FakturPajak_Master] CHECK CONSTRAINT [FK_FakturPajak_Master_Trade_Master]
GO
