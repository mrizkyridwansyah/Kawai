SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NotaRetur_Detail](
	[NotaRetur_No] [char](15) NOT NULL,
	[FakturPajak_No] [char](25) NOT NULL,
	[Item_Code] [char](25) NOT NULL,
	[ReturnSeq_No] [numeric](4, 0) NOT NULL,
	[Return_Date] [datetime] NULL,
	[Qty] [numeric](18, 5) NULL,
	[Unit_Qty] [char](2) NULL,
	[Currency_Code] [char](2) NULL,
	[Price] [numeric](18, 5) NULL,
	[Adj_Price] [numeric](18, 2) NULL,
	[Amount] [numeric](22, 5) NULL,
	[User_Entry] [char](15) NULL,
	[Date_Entry] [datetime] NULL,
	[User_Update] [char](5) NULL,
	[Date_Update] [datetime] NULL,
 CONSTRAINT [PK_NotaRetur_Detail] PRIMARY KEY CLUSTERED 
(
	[NotaRetur_No] ASC,
	[FakturPajak_No] ASC,
	[Item_Code] ASC,
	[ReturnSeq_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
