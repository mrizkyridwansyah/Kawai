SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [AR_Detail](
	[Cust_Code] [char](15) NOT NULL,
	[AR_No] [char](12) NOT NULL,
	[Invoice_No] [char](25) NOT NULL,
	[Currency_Code] [char](2) NULL,
	[Amount] [numeric](22, 5) NULL,
	[PPN] [numeric](18, 0) NULL,
	[Exchange_Rate] [numeric](9, 2) NULL,
	[Exchange_Amount] [numeric](22, 2) NULL,
	[Last_Update] [datetime] NULL,
	[Last_User] [char](15) NULL,
	[Register_Date] [datetime] NULL,
 CONSTRAINT [PK_AR_Detail] PRIMARY KEY CLUSTERED 
(
	[AR_No] ASC,
	[Invoice_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [AR_Detail]  WITH NOCHECK ADD  CONSTRAINT [FK_AR_Detail_AR_Master] FOREIGN KEY([AR_No])
REFERENCES [AR_Master] ([AR_No])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [AR_Detail] CHECK CONSTRAINT [FK_AR_Detail_AR_Master]
GO
ALTER TABLE [AR_Detail]  WITH NOCHECK ADD  CONSTRAINT [FK_AR_Detail_Trade_Master] FOREIGN KEY([Cust_Code])
REFERENCES [Trade_Master] ([Trade_Code])
GO
ALTER TABLE [AR_Detail] CHECK CONSTRAINT [FK_AR_Detail_Trade_Master]
GO
